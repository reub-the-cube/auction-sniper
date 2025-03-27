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

### Activity 11.2 - Fixing the failures

#### Allow UI elements to be found by the Appium selector
```
OpenQA.Selenium.NoSuchElementException : An element could not be located on the page using the given search parameters.; For documentation on this error, please visit: https://www.selenium.dev/documentation/webdriver/troubleshooting/errors#no-such-element-exception
```
This is simple enough. There is no element with the `AutomationId` property of `AuctionId`, as required by the test. This happens two more times because the test also interacts with the 'Join Auction' button, then tries to retrieve the 'Sniper Status' before its first assertion.

[This](https://github.com/reub-the-cube/auction-sniper/tree/177a9e93234e5b9449608d94cb5fe83b69cb91ef) is how the repository looks after this step.

#### Get the sniper to send a join message to the XMPP server
```
Expected SniperBiddingStatus() to be "Joining" with a length of 7, but "" has a length of 0, differs near "" (index 0).
```
This is clear too. The status should now be joining, but it's not set. The simplest possible thing to make it pass would be to change the UI to 'Joining' but that's a bit short-sighted because the test will not go green as we're doing the skeleton and expect further changes and assertions against this value.

I start by connecting to the XMPP server from the application (aware that in the 'real' flow, the auction will have started selling) because it is the action of sending the join request that will get me closer to green.

I seem to connect magically but then start changing the values and realise it's not connecting correctly. It was because I wasn't awaiting the connection to be opened, so need to run the method asynchronously. Then the validation errors begin, and I can work through them (trusting Openfire certificates, and a password change due to a character I had troubles with).

I get the test passing for Windows, but skip the certificate validation for Android. I then have some strange quirk with `FindElement` on Android not picking up the element, but resolve that after trail and error by moving it out of the layout and then back in again!

I was ready to commit here, but realised my Openfire details were in plaintext. The other examples online have this too, but I want to be more secure about it, so I set up some Application Settings, using [this blog post](https://montemagno.com/dotnet-maui-appsettings-json-configuration/) for inspiration.

[This](https://github.com/reub-the-cube/auction-sniper/tree/fef2057406ad9a7340bec34347f40d941b339963) is how the repository looks after this step.

#### Get the auction to notify the sniper that the auction has ended
```
Expected SniperBiddingStatus() to be "Lost" with a length of 4, but "Joining" has a length of 7, differs near "Joi" (index 0).
```
This failure needs a bit more thinking but I need the status for the sniper to be 'Lost'. The behaviour I want is for the app to be notified of the auction closing for the item. So I'll need to communicate with the sniper from the auction server. The auction will need to know that the sniper has joined to do this in order to send the notification.

I want to start with the auction sending a message that it's closed, since that is what the sniper needs to know. I want to separate my username and password out of the code, so I create a `testsettings.json` and `testsettings.dev.json`. All platforms will need this for the test so it goes in the shared project, and to get the tests to find the resources, they need to be links from each of the platform-specific projects - and copied to the output directory. This also results in the `BaseFixture` class being created to handle platform-agnostic fixture work.

I really struggled to get the app and the fake auction communicating. I was doubting my understanding of the messaging system so I stripped back everything and created two separate console apps (one as the sniper, and one as the auction item) that connected to the server, and they could send messages to each other. Eventually I realised it was all down to a typo. But I picked up some useful learnings along the way, such as sending the message after binding and that you can't received messages if you don't have the correct presence on the server. 

I am doubting the choice of XMPP client (the `xmppdotnet` NuGet package) as the documentation is lacking but it is the recommended dotnet client on the Openfire site. I also expect that some changes will have happened in the 15 years or so since the authors made the decisions and wrote their code, so naming conventions and features have moved on a bit.

Typo resolved and decisions in the past, I get back to the task in hand and the auction server can receive the sniper's message which declares they're joining the item's auction. I need to handle the message to store the sniper's username, so I can send the message back when the auction closes.

I don't see the value in the naming of `HasReceivedAJoinRequestFromSniper`. The method doesn't do what it says because it doesn't check for a join request specifically, or that it's from sniper, so I find this misleading. I think `HasBeenJoined` is a better name for the test, and showing intent to the user. I'm also not a huge fan of assertions outside of the test specifically (although this assertion is done in the framework, in `FakeAuctionServer`). 
```
auction.HasBeenJoined().Should().Be(true);
```
The above reads more clearly to me. Maybe this will change as the chapters unfold. I regret not changing this earlier and it would have had a different test failure sooner.

That assertion passes and I have some issues with the UI and threads, so bring in a binding context and view model to handle the update of the status to 'Lost'. It's green, we have our first vertical slice and I might do a bit of tidying up before starting chapter 12.

[This](https://github.com/reub-the-cube/auction-sniper/tree/cf6780453e4615dd92549be47ab7ffb9a81005e7) is how the repository looks after this step.

### Reflecting on Passing the First Test
I look back over the code and review the differences I have locally to the book in Chapter 11. 

|Difference|In the book|In my repo|Impact|
|---|---|---|---|
|Starting the application|The book runs that application, initialising it with the sniper's, XMPP server and `itemId` details.|I run the application, load the sniper's and XMPP server details and type the `itemId` through the UI.|I don't think my implementation is 'different' but it is slightly more complex. It was probably due to the default project containing some `xaml` markup ready to go.|
|Driver and WindowLicker|The book has a prober to check on the on-screen components|I have a fixed delay per platform, but can't repeat the check|My version may be a bit more brittle as it'll only check once, but behviourally the app is the same.|
|Fake auction initialisation|The book's Smack client is different from the Xmppdotnet client, and has different APIs. It logs in and has a chat listener and a message listener.|I have a message listener for the user receiving a new `Message`.|I might be missing a trick with something in the library the manages 'group' chats, but it'll flush out later.|
|Message listener|The book uses a blocking queue and just receives one message of any type|I receive multiple messages and treat them differently, and I have the same listener for the auction and the sniper.|I know if there's been specifically a join request. I have extra logic because of the shared message listener.|
|Sniper receiving message|The book uses the Swing utility `invokeLater` which uses some Java magic to run on the AWT event dispatch thread.|I am using the event listener as mentioned above, and checking for specific message data. I am not just setting the status to 'lost' when any message is received.

I think using a shared message listener might have been too far. Perhaps it'll need separating. The message listener has code added for the auction and the sniper, which is merged together but the handling of both is actually unique.

## Chapter 12 - Getting Ready to Bid

### Activity 12.1 - A Test for Bidding

The next part of the behaviour is the sniper bidding. It took me a couple of reads to get that bidding does not mean the sniper has made a bid, but that the auction has reported a bid (see page 78). However, the test asserts that the sniper sends a higher bid but still loses. For me, this isn't an acceptance test of much value because it's not a real journey for the sniper.

With the scaffolding in place adding the next acceptance test is much simpler to get into place. The first thing that I notice is the different approach for the auction asserting against the messages received.
```
auction.HasReceivedBid(1098, sniperUser.Username);
```
Unlike in the previous chapter, with the `HasReceivedJoinRequestFromSniper` method, which didn't pass any arguments through, this is specifically checking the content of the bid message and who it's from. The book remarks on this difference as I did above, and I will go back to assert that the join message is from the sniper when I'm next green.

They also choose to extract the message formats, which is a step I started to take in the first chapter to remove my duplication early.

The test fails for the expected reason as the implementation isn't written yet.
```
SniperBiddingStatus() should be "Bidding" but was "Joining"
```

[This](https://github.com/reub-the-cube/auction-sniper/tree/35567a38fe01566ea186ce0296fe2e3cd19131c8) is how the repository looks after this step.

### Activity 12.2 - The AuctionMessageTranslator

In order to get my working skeleton, I've already introduced the concept of the translator (which is the MessageListener class), which inteprets the messages from the chat and sends the close message via an event to the UI. This section builds on the idea, and gives the confidence through tests. I probably should have written these tests at the time given it is controlling behaviour.

#### A decision on asserting events and verifying invocations
My implementation was to send the events directly to the UI, but the book introduces an `AuditEventListener` class which can be mocked to check a method is invoked rather than asserting on an event being raised. I decide to try both in parallel (with the unit test of course). [Here is the comparison](https://github.com/reub-the-cube/auction-sniper/commit/d021b009968e5b0f070e32217082c59902a7ec02) between the two approaches. There is a neatness about the mock that isn't there with the event handler in the tests, so I will try to keep aligned to the book here.

#### Changing tack to align on invocations
As I do this, I decide that it seems to me the sniper and the auction house will translate messages differently. For example, the auction house needs to know when a client joins, and the client needs to know when the auction ends. There may be shared translations too, but by separating these at this point, I can keep my tests green without creating an auction event listener specifically for the tests (as it's not yet required).

At this point it is still on red for the acceptance test (making a higher bid), but green on the first unit test for the auction closed event.

[This](https://github.com/reub-the-cube/auction-sniper/commit/eb63800f0502c0e46ce762a88844843bbd32f2e0) is how the repository looks after this step.

### Activity 12.3 - Unpacking a Price Message

The second test for a different auction event is simple enough. With the [Moq](https://www.nuget.org/packages/Moq/) library, which was added for the first unit test above, you can choose between `MockBehavior.Strict` and `MockBehavior.Loose`. A strict behaviour will (prior to correctly implementing the production code) fail due to the fact that the `AuctionClosed` method is invoked, but not setup. A loose behaviour will only verify against the setups you have created, will fail the test due to the fact that the `CurrentPrice` method has not been called.

I included two commits here, though it could have been more, jumping over an element the book skips around the obvious implementation. The first commit has the values of the test in the production code, but the second parses the message properties to send the correct parameters to the `CurrentPrice` method.

[This](https://github.com/reub-the-cube/auction-sniper/tree/64fde75f7af435999f7cd1c7be0913da7681c72f) is how the repository looks after this step.

## Chapter 13 - The Sniper Makes a Bid

### Activity 13.1 - Introducing AuctionSniper

This activity introduces another layer for handling the auction events, to allow updates to go from the `MessageTranslator` to the `AuctionEventListener` to a `SniperListener`, which is bound to the UI. With the .NET MAUI project, I have to create a new class library to be able to reference the `AuctionSniper` from the tests and the MAUI app project but that is simple enough.

### Activity 13.2 - Sending a Bid

The circular dependency issue from the book isn't a problem for my work, so I add an implementation of the `Auction` interface locally to the Main Page, expected it to move shortly. My XMPP client takes the user as an argument to the message, which is different from the Smack API in the book, so I'll need to think about that soon. The end-to-end tests are green, with the status updating to 'Bidding' and then to 'Lost' when the auction closes. 

[This](https://github.com/reub-the-cube/auction-sniper/tree/d3845bfa00ab5a4f6ecd5536dd3e7c29c0e4999b) is how the repository looks after this step.

### Activity 13.3 - Tidying Up the Implementation

I had already moved the auction class out of the noise of the main page, which is what happens next in the book. I decide to add a `Join` method to the `Auction` interface to enforce the concrete type to join the auction. I also wonder about how it'll work with multiple auctions, so I re-work the MainPage to handle the `Auction` at item level, rather than page level. Not sure how this will develop yet, but it actually moves the structure of the code closer to what is in the book for now.

I already have the SniperListener implemented away from MainPage, on the `MainPageViewModel`, which might need more work later but I'm OK with that for now. I choose not to refactor the `SniperTranslator` as I don't think we have a use case that calls for it yet, and it is clear to read.

## Chapter 14 - The Sniper Wins the Auction

### Activity 14.1 - First, a Failing Test

Adding the new end-to-end test is simple enough, which builds on the previous one by asking the option to report the price that the sniper has bid and opening the way for 'Winning' and 'Won' statuses. When I write this, I notice that the tests doesn't know that the auction knows it has been joined by _the sniper_, which feels important. I might look at this when we get back to green.

### Activity 14.2 - Who Knows about Bidders

It's a minor difference but I update my unit tests before altering the production code at all, even if it doesn't complile. Write a failing test, make it compile, make it green. This isn't a blog post about Test-Driven Development, but I [created a branch](https://github.com/reub-the-cube/auction-sniper/commits/activity-14.2-who-knows-about-bidders/) with a step-by-step, near-to-one-line commits (14 of them) on the changes I made to add the `PriceSource` argument to the `CurrentPrice` method.

### Activity 14.3 - The Sniper Has More to Say

Unit tests need adding before working through the logic back to the Application and the `SniperListener` will need to be told the sniper is winning.

### Activity 14.4 - The Sniper Acquires Some State

Updating the current unit tests makes sense, but I do this one at a time. Allowing, expected and ignoring are similar concepts to the `MockBehavior` mentioned above. Before making the change, I am only verifying against specific invocations I specify, so - for example - `ignoring(auction)` is already satisifed. This would not have been the case if I were mocking `Auction` with `MockBehavior.Strict`.

When I consider the `States` syntax, I don't have an immediate equivalent to call upon. `Moq` allows you to add callbacks and create sequences. I think the same can be achieved here, although I am concerned about coupling the test too closely to the code. The assertion is that if the sniper is bidding when the auction closes then it will be lost, but it is only bidding after a price has been reported from another bidder.

So far, I have only used the `Verify` option with a expression on the object under test for assertions. I choose to go with a `MockSequence` here, which requires `MockBehavior.Strict`. I ensure the last method in the sequence is called at least once, to verify that it has happened, to wrap up the test. I will see where the tests go, but an alternative would be to assert/verify after each action e.g. _when_ reporting a price _then_ the sniper is bidding, _when_ the auction closes _then_ the sniper has lost.

[This](https://github.com/reub-the-cube/auction-sniper/tree/cb8d1c156a93b61ed357e73816da05cdc29aa0a2) is how the repository looks after this step.

## Chapter 15 - Towards a Real User Interface

### Activity 15.1 A More Realistic Implementation

I opt to use a [ListView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/listview?view=net-maui-9.0&source=recommendations) for presenting the data. The test side is already quite neat, from using the `AutomationId` attribute on a XAML element, so I just need to ensure the status field of the list view has the correct id.

I added a list of snipers alongside the existing sniper. I found that when updating the property of the sniper with the bid status, it wasn't reflected in the app when just using an `ObservableCollection`, so needed to raise the `OnPropertyChanged` event as was being done with the original sniper. I did this by using a combination of the dotnet samples, and [an easy-to-understand mini tutorial on `ObservableCollection`](https://wellsb.com/csharp/maui/observablecollection-dotnet-maui).

I then remove the bindings to the original sniper, and the UI is now tied to the first item in the list of snipers. There's an piece on the to-do list for handling multiple items so I'll tackle that problem then.

### A detour for test interdependency

I think there's a dependency in my tests, because the always pass in isolation but often fail in a group. I expect it's holding onto the chat and not resetting the status from the sniper's perspective or something. The app isn't reloaded for each end-to-end test so there must be some lingering state that I should tidy up.

I could remove the Fixture from the test collection, but from what I understand the driver could be the same and control the app. I blunder around a little - some ideas and APIs online for Closing / Launching / Starting / Quitting the app are for different frameworks or have been deprecated.

I choose to update the `BaseTest` to implement `IDisposable` which will execute for _each_ test, not every collection, which the idea to relaunch the app from the driver after each test has finished. I can call the PlatformTestFixture from here (which perhaps creates a bit of a blurred line for the true definition of a fixture) to add some platform-specific code for relaunching the app.
```
// Windows
App.Close();
(App as WindowsDriver)?.LaunchApp();

// Android
(App as AndroidDriver)?.StartActivity(<AppPackage>, <AppActivity>);
```

Now the app re-opens after each test, giving a clean starting point for each one.

### Activity 15.2 - Displaying Price Details

I spend a bit of time thinking about this. The idea is to add the item identifier, the last price and the last bid. The last bid is interpreted differently by the authors to me. In this excerpt from the `sniperWinsauctionByBidderHigher` end-to-end test
```
[...]
auction.reportPrice(1000, 98);
application.hasShownSniperIsBidding(1000, 1098); // last price, last bid
[...]
```
I think the last bid is either `1000` (made by "other bidder"), or nothing at all. Given it's the Sniper's view of the auction, they haven't yet made a bid, so I think it should be null. _After_ the auction reports a price _from the sniper_ then we should test that the sniper is winning. Thinking about it a bit more, if the auction house doesn't respond in real-time, it could be useful to return what has been bid (as opposed to what has been accepted by the auction). In the code this does happen this way; `auction.Bid()` before the `sniperListener.Bidding()` notification so I have talked myself around by thinking it through!

I'm working out how the controls of the markup work, and struggle to get the spacing working as the cells went on top of each other. The starter docs use a fair amount of `Width=Auto` but [this article prompts me to use `Width=*`](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/grid?view=net-maui-9.0#rows-and-columns), which is also the default value, and makes it look organised at least.

The `SniperState` brings together the itemId, current price and last bid. I consider why it doesn't bundle up the status too in the value type. I suppose by keeping the listener events more declarative (`onBidding`, `onWinning` etc) it stays easier to read. I could also suggest that each sniper listener notifcation doesn't require all the state properties - for example when the auction closes, it's only the status that will have changed.

When writing the tests for `SniperState`, when the value of the object is not of relevance to the test, I use the `Moq.It` type to allow any value through i.e. `It.IsAny<SniperState>()`.

I add a unit test for the app, which checks the value of the properties on `Sniper` are set and the `OnPropertyChanged` event is raised to notify the view, and need to update the project properties of the App and the Unit Tests to be able to reference each other. Having the App project as a dependency for the unit tests makes them slower while it builds; I might extract them later.

I have made quite a few changes, so commit on a red end-to-end test. It's failing (as in the book) because the price of 1000 hasn't been updated; this is because the sniper state isn't set for the other auction events (winning, won, lost).

### Activity 15.3 - Simplifying Sniper Events

This starts off by remarking on what I had spotted above - which is that the status should be bundled into the sniper state. It starts with a renaming from SniperState to SniperSnapshot, so that the words state and status are less easily confused - I like this.

Updating the tests is simple enough, although one part catches me out as I have a each test needs to report the current price twice to get the sniper to winning (following the state machine). Without first bidding, the winning snapshot will have a bid price of 0.

[This](https://github.com/reub-the-cube/auction-sniper/tree/560858e478650bbd213c3ce8603712d1a47e53c0) is how the repository looks after this step.