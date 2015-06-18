# Cycle London

####I won 2nd place! A big thanks to the guys over at Programmr
http://www.programmr.com/hired_london_hackathon

This app project may be deprecated in favour of the Windows 10 Successor. [Please visit the repo ](https://github.com/lewisj489/LondonUniversal-Win10)
#### Making cycling in London easy


###Important links
* [Promotional Video](http://youtu.be/-z3iZvoW3kw)
* [Store App](http://apps.microsoft.com/windows/en-gb/app/cycle-london/a0964f6e-a9b8-4079-90c5-97307569b4a9)
* [Phone App] Not ready
* [Website](http://lewisj489.github.io/Cycle-London-Universal/)


***Please note*** that the *website* and *video* was only made for fun and should not be taken critical.

###What is Cycle London
####What?
Cycle London is a Windows universal app. 
The app(s) main goal is to help tourists and commuters find a Santander Bike point in London with ease.
The Windows store app is for Windows 8.x, and Windows Phone is for Windows Phone 8.1.
The app will likely be upgradeable to Windows 10 universal apps, with some minor tweaks.

####Who?
Me, Lewis Johnson. It says it right there.

####Why?
[Short answer](http://www.programmr.com/hired_london_hackathon)

These apps, or this app? I'm not sure. The code base is so different it might as well be 2 apps, I digress. 
Anyway this app was made for a Hackathon over at [Programmr](http://www.programmr.com/hired_london_hackathon]),
Which was sponsored by [Hired](https://hired.com/?utm_source=programmr). I would like to thank both of those companies, because otherwise this app wouldn't have existed. I have never made an app before, or anything with a meaningful UI. Without those awesome guys, I would have never tried to make one.

####More?
######Ramble Warning!
Just a bit. I'm just going to babble a bit. This project started off as an Android app, then it went to Windows Phone, then Android, then Universal, then Android, then I settled with Universal. This whole thing has been a real challenge to me, but I've had a great time making it. I tried to stay of site like StackOverflow and Reddit while I was making this app so I could get a feel of what it's like to be a professional, and it hurt. This app was actually targeted as a Windows Store app, and then I later on added the phone version. I like the whole hub UI this app has going for itself, but porting the UI to phone was a real struggle. I don't even use mobile apps much so designing a friendly UI was mission impossible.

####Future?
Short answer: Triumphant comeback with Windows 10.

I really like what I've managed to come up with, but I didn't have long to make the app at all. 
I am currently re-writing the whole app! Yes, bonkers right. I really like what Microsoft is doing with Windows 10 and I’m jumping on the **NEW** universal apps bandwagon. Windows 8.x universal apps are ok, but they don't share enough code. With Windows 10, the maps, list and search are all the same controls. I should be able to use at least 80% of the same code. With this app the XAML styles are a million miles apart, with Win 10 that won't be the case.


###Building for testing
I understand you are professionals, but I thought I should include this just incase. I am deeply sorry that I could not upload the phone app to the store on time, but Microsoft can take up to 7 days to verify apps. I'm guessing you don't have a Windows phone either, so you can use the emulator with Visual Studio. To select a startup project do as follows.
![Config select](http://i.imgur.com/1LP2n3k.png)


####Windows Store app
To build for Windows 8.1 you will need 
* Visual studio 12, 13, 15CTP or 15RC (Community editions are fine)

Bing maps has some special requirements, to accommodate, Please select x86 DEBUG in the build configuration. If you don't the build will fail. Visual Studio *may* ask for a key, if it does continue reading.

Making a temporary key -
In the "Windows 8.1" project open the Package manifest. Select the Packaging tab. Next to publisher select the "Chose certificate" button. A new menu will pop-up, enter the details (They can be whatever you like) and then rebuild.
![SetupImage](http://i.imgur.com/GptxM7N.png)
![SetupImage](http://i.imgur.com/LB6NgzW.png)


####Windows Phone
It is **strongly** recommended that you try the *Windows app* before the Windows Phone app. The reason being that it is more polished and you will get a feel for the UI/UX and app's features.

To build for Windows Phone 8.1 you will need either 
* 64-bit version of Windows with Hyper-V
* A Windows Phone 8.1 which is [developer registered](https://msdn.microsoft.com/en-us/library/windows/apps/ff769508(v=vs.105).aspx)

You will also need Visual Studio 12, 13, 15CTP or 15RC (Community editions are fine).
To build you simply have to open the Cycle_London solution and build the Windows Phone 8.1 project. If you are using a physical phone remember to select it instead of running an emulation.
![Config select](http://i.imgur.com/OAFaAbu.png)



