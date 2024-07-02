Some intro on why I'm doing this and what I hope to achieve.

## Chapter 9 - Commissioning an Auction Sniper
I choose to build my Auction Sniper application using the .NET Multi-platform App UI framework in Visual Studio. My personal device is a Windows machine, I have some C# experience and have a couple of ideas for mobile apps that I've never followed through with so this exercise will give me a good teaser into what it might be like working with .NET MAUI.

I start by [updating my Visual Studio install](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-8.0&tabs=vswin) to include the .NET MAUI workload, then create a new solution with a default .NET MAUI App project and initialise [my GitHub repository](https://github.com/reub-the-cube/auction-sniper/).

I read through - as is often the case with a book - the rest of the chapter, and familiarise myself with the initial deisgn, the sequence of features we want to build, the notes on the XMPP communication layer and the disclaimer about it not being real.

## Chapter 10 - The Walking Skeleton

### Activity 10.1 - Our Very First Test

The book starts with an end-to-end test. I will need a way of checking how the application and the auction are behaving. UI testing tools are a bit lacking, but I plan to start by using Appium with xUnit and follow [this Microsoft devblog](https://devblogs.microsoft.com/dotnet/dotnet-maui-ui-testing-appium/) as a rough guide.

I could follow the book dogmatically and write the test before deciding on my 'end-to-end' tooling, but I'm going to verify I can do that before writing the test. In other words, since I haven't used Appium before, I want to get an _even_ simpler test working so I'm going to work off the blog until the tests pass then revert to the book when I have a working base.

The out-of-the-box project for the App conains more that I need right now. It's an image, a heading, a sub-heading and a button. The button has a click event handler which dynamically updates the button text. The article mentions adding an `AutomationId` attribute but I don't think I need to do that yet and I'm not quite sure what I need to test. But it's good information to have.

I have node on my machine already (v20.15) - but it might be important to remember for any CI - so run `npm i --location=global appium` and install the drivers that are listed in the blog, including the older version (1.2.1) of the WinAppDriver. There's a few module dependencies which are no longer supported which is a bit of a red flag but I also know there aren't a lot of options and this isn't production code.

I start by cloning the [sample project](https://github.com/dotnet/maui-samples/tree/main/8.0/UITesting/BasicAppiumNunitSample) and copy the Shared, Android and Windows projects across to my solution. The tests don't pass. I'm glad I tried it this way first before confusing matter with the actual domain problem.

### A detour for Windows tests

I decide to focus initially on trying to get the Windows tests working, and will refactor to xUnit once I know what's going on a little bit better.

The exceptions I get are of the pattern:
```
OneTimeSetUp: OpenQA.Selenium.WebDriverException : An unknown server-side error occurred while processing the command. Original error: <An error message>
```

I didn't keep track of the error messages and the changes at each step but a couple of them were:
- Package was not found. (Exception from HRESULT: 0x80073CF1)
- The system cannot find the file specified
- Failed to locate opened application window with appId: <path_to_app>, and processId: <process_id>
- Value does not fall within the expected range

I explore various combinations of the `App` property on the `AppiumOptions` object with various properties and settings on the project options and manifest files, try installing and uninstalling the app to my local app store, read around the documentation a bit, experiment with starting the Appium server outside of the test - the Appium drivers and clients are all new to me - but have no luck. Eventually I decide to physically run the app, and then run my tests with various `App` values... and two of the three pass!

The two tests that pass are virtually identical. They just take a screenshot of the app. The failing test cannot find a UI element and that is because I knowingly neglected to add the `AutomationId` attribute. Stick it in, and the test passes.

It still doesn't feel like the project is set up perfectly. I feel like the driver should start the app itself under its own process (since it closes it down). A couple of articles indicate the Windows driver may have trouble starting the app so I was prepared to accept this when I stumbled across the correct syntax for the Application ID. All of the `App` combinations I played with earlier were apparently not exhaustive.

It starts working with `com.companyname.app_<publisher_id>!App`. I had something similar to this from after I had publish the app locally; the next local run I did from the IDE told me I'd be replacing an install. But that application information also included version and device configuration data.

I find the `com.companyname.app` configuration in the MAUI Shared section of the project properties. `<publisher_id>` takes a while but I knew it must have been a part of the publish and I find it hidden away as a read-only field in the Packaging tab of the `.\Platforms\Windows\Package.appxmanifest` file. Searching the solution for my Publisher ID yields no results! I assume that the `!App` part of the Application ID related to the App part of the package.

I adjust the settings, the changes don't seem to take immediate effect so I also re-publish then un-install the old one and run locally from the IDE which seems to resolve some conflicts. Now my Application ID is `com.madetechbookclub.auctionsniper_<publisher_id>!App`, and it all feels quite neat.

[This](https://github.com/reub-the-cube/auction-sniper/tree/22f1ed324e5f05a8964fc380d8d5eddad9d88c03) is how the repository looks after this step.

### A detour for Android tests

I check out the error message for the first failing Android test and realised I need the Android SDK. The easiest barrier to entry is to install Android Studio, so that's what I do. Then I set up a device - a Pixel 6. A couple of minor changes to the test (e.g. the package name) and I to deploy the app to my new local device. It works! 

It was a lot less painless than the Windows app but some things had already been ironed out.

[This](https://github.com/reub-the-cube/auction-sniper/tree/3e608382ee4b61044d6839c534b382d09c0ac48b) is how the repository looks after this step.

### A detour for xUnit

I have Windows and Android tests passing (on the OOTB 'click me' project) and am ready to switch from NUnit to xUnit. The frameworks have their differences. Mainly there is no direct equivalent of `[OneTimeSetUp]` which is where the Appium server starts and the drivers are initialised.

I start the refactoring for the simplest test possible with that on the backburner. The `AppiumSetup` class (one per platform) is renamed and tweaked to suit xUnit (constructor and disposal methods vs setup and teardown attributes). 

I then bring the tests back together by making use of the `ICollectionFixture<T>` interface for per-test-collection data. The name of the collection is an attribute on the test class, which can be inherited, and reference a shared property on the platform-specific fixture (what used to be called `AppiumSetup`), so that each dependency is unique per platform and only created once.

Idempotency is important with unit tests especially but as the start-up time for the Appium server / emulators / drivers is slow, it makes sense to just start the app once per suite of platform tests. I can always perform restorative actions on the app later if necessary.

Although the changeset has a fair number of changes, the main changes are:
- Define a collection definition per platform
- Add the name of the collection to the fixture (where platform-specific code already lived)
- Add the collection attribute to the `BaseTest` class

[This](https://github.com/reub-the-cube/auction-sniper/tree/7226ebf5a745719c3ef4fd127255c7e33ae8e31f) is how the repository looks after this step.

### Activity 10.1 continued - back to Our Very First Test

It's taken several hours to get here, but is case-in-point for what the book explains as the initial curve to get set up with the Walking Skeleton. Better to do it now (in iteration zero) with minimal dependencies that a few weeks down the line!

This end-to-end test has an auction that starts, the sniper joins (requested and received), the auction closes and the application shows the sniper lost. This will be a shared test because that should happen on all platforms.

I keep the `AppLaunches` test because it feels like something lightweight may be useful at some point, but remove any other tests and create a new one, which is obviously failing, called `SniperJoinsAuctionUntilAuctionCloses`.

[This](https://github.com/reub-the-cube/auction-sniper/tree/f789ae7246877fb70451740bbd4f3172af829369) is how the repository looks after this step.

## Chapter 11 - Passing the First Test

### Activity 11.1 - Building the Test Rig
I kept the infrastructure the same with the idea to use Openfire for the messaging, and I installed it with an embedded database. Set up users, as per the book (but with more secure passwords!).

The Application Runner code block controls the application a bit more that I require it to, so I keep the app instructions in the test class for now.

The book talks about the minimal implementation, but I prefer to get the code compiling as soon as possible, so cut even more out (knowing, for example, that adding the messaging is still required).

### Activity 11.2 - Fixing the First Failure

```
OpenQA.Selenium.NoSuchElementException : An element could not be located on the page using the given search parameters.; For documentation on this error, please visit: https://www.selenium.dev/documentation/webdriver/troubleshooting/errors#no-such-element-exception
```
This is simple enough. There is no element with the `AutomationId` property of `AuctionId`, as required by the test. This happens two more times because the test also interacts with the 'Join Auction' button, then tries to retrieve the 'Sniper Status' before its first assertion.

```
Expected SniperBiddingStatus() to be "Joining" with a length of 7, but "" has a length of 0, differs near "" (index 0).
```
This is clear too. The status should now be joining, but it's not set. The simplest possible thing to make it pass would be to change the UI to 'Joining' but that's a bit short-sighted because the test will not go green as we're doing the skeleton and expect further changes and assertions against this value.

