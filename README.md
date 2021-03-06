# RaspberryPi.CSharp

# Overview

This repository provides information about the Raspberry Pi.  It was specifically for those that participated in a Money20/20 hackathon for the Vantiv API challenge but now it is an excellent artifact to support learning how to write code and leveraging a payment API for the Pi.

Infrastructure in the financial world is mind numbing and detecting issues quickly is a high priority for platform teams.  What if we could send each one of the merchants, or financial institutions, a cheap device that would help us detect processing issues before they happened?  Enter the Raspberry Pi!

This code is going to integrate to the Vantiv platform, using the Mercury SOAP API, and periodically send a transaction.  The code will then run some quick analysis on the results and flash a green or red LED based on that analysis.  As you will see the analysis is trivial in the code but this is a small step to what could be an actual product.  Think about the ability to have a low cost processing platform within each merchant environment...the opportunity is vast!

# Unboxing the Raspberry Pi

This repository leverages the follow kit : http://www.canakit.com/raspberry-pi-starter-ultimate-kit.html.

The ultimate starter kit from CanaKit seemed to be a good fit for learning about the Pi.  The kit has everything needed to get up and running quickly and has enough extras to allow for creativity.

Here's the initial packaging and the contents after opening:

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/raspberrypibox.JPG)

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/raspberrypiboxafteropening.JPG)

For those of us that do not work with hardware the contents look daunting.  All I know how to do is type.  This looks dangerous.

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/geekycontents.JPG)

These boxes looked easier to manage.  Rectangles and cardboard...I can handle that.

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/boxesinsideofboxes.JPG)

A quick start guide and more scary looking components.  What is that silver thing?  And that picture on the quick start guide looks like that one time I had to take the case off of my computer and blow out all of the dirt/cat hair to make the computer start again.  Well, at least there is a guide, I rarely read documentation but it was included so it might actually prove useful.  If nothing else maybe it tells me how not to accidentally electrocute myself.

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/yinandyang.JPG)

Well, that was not so bad...except for the fact that I did not have an extra keyboard or mouse lying around.  A quick trip (one of many) to the local Best Buy would solve many issues.

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/allpluggedin.JPG)

# Setup

Why am I telling you about Raspbian inside of a c# GitHub repo?  Well, I had to start somewhere and while the wires are a little frightening a linux variant does not scare me.  There are quite a few getting started guides (go ahead and google and you'll find them) but it looked like most of them are for Raspbian.

I started with Raspbian, made everything work with Raspbian, and then switched to Windows IoT to see the differences.  Turns out, not much difference.  If you are interested in the Python version of this code click on over to the python repository.  After we get through the two Raspian setup screens we'll switch to Windows and c# for the rest of this loooong "readme".

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/settingupraspbian.JPG)

Setup screens are usually boring but I found myself engaged while reading each of the setup screens for Raspbian.  Maybe there were subliminal messages telling my body to get into the hardware groove?  As I watched/waited I started peering into the box and showing the cables no fear.

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/almostthere.JPG)

As for Windows IoT it was similarly easy to setup and get going.  I followed the Getting Started guide at this link:  https://dev.windows.com/en-US/iot.  Basically you copy the Windows IoT core image to a micro sd card (must have Windows 10 to do this) and then insert the micro sd card into the slot on the Raspberry Pi.  Everything went smoothly and before I knew it I was sitting there looking at the screen thinking, "what should I do now?"  But that did not last long, I downloaded the Blinky sample code, compiled it, struggled for a few minutes trying to determine how I deploy, and then found the magic:  the Windows IoT Core Watcher.  This little app runs in the background and when a Raspberry Pi is attached it will be discovered and then using an infamous right click it is possible to login via a web browser and start poking around.  Unfortunately the watcher does not find Pis with USB WiFi dongles.

All of the same information is available if you use the HDMI cable and work on the device itself but it seems cooler to work remotely and just have the little Pi sitting there in its uncomplicated glory.  Now that I gained access to the device itself it was time to write an application.


# Deploying

Well, not quite ready for code.  Everything is setup and ready to go, so let us first discuss deployment.  I had the Blinky sample code loaded in Visual Studio.  If you look on project settings then click the Debug tab you will see the Start options.  Make sure 'Remote Machine' is selected and then type the IP address of your Raspberry Pi.  Hmmm....how are all of the participants going to know what the IP address is of their Pis?  This is where the HDMI cable comes in handy...plug the HDMI cable into the Pi and then into your computer and you should have everything you need.  After you get the IP address you can unplug your HDMI cable and go back to coding.  Do not check 'use authentication'.

Hit the F5 button and that will deploy the Blinky application to your Raspberry Pi and after a short couple of minutes the LED will start flashing.  Your 'Hello World' application is now complete.

# Writing Code

Now it's time to write some quick code to integrate to the Vantiv API.  We will use the Blinky code as a reference which basically makes an attached LED blink but we're going to use two LEDs (a red and green one) and we will make them flash quickly for two different use cases:

* If the transaction approves we will make the green LED flash otherwise it will flash the red LED
* If the elapsed time of the transaction is > .5 seconds then we will flash the red LED otherwise flash the green LED

These two things will set us up nicely for future product features like a web server running on the Pi that we could access to look at analytics or we could send this data (along with other useful information) to a cloud server and perform analytics there.  The use cases are limitless!

The code is fairly straightforward:

* We moved the relevant GPIO controller code to the CustomGPIOController class.
* The access to the Vantiv API is in MercuryAccess.cs.  The integration is as simple as adding a reference to the SOAP API and then writing a quick XML string to send transactions.  We could create all sorts of modifications to the XML string but for this demo, and to keep things simple, we hard coded most of it.  If we were creating a real product we would likely want to test many different transaction types, and save/report on statistics for those different types, but at the same time we do not want to flood the test environment (although that would be another good test).
* Finally the main code is in MainPage.xaml.cs.  This project does not have much of a user interface, mainly because no one will see the UI.  You will see that the initial work is in the constructor of the class and the constructor starts a timer where the rest of the work is accomplished.  The code became a little ugly and needs some refactoring but that's what happens when we're hacking away for fun.  We do love to hear feedback though so send it our way and/or submit a pull request!
* You will see code to write to a local file.  That code was non intuitive so we thought we would leave it in.  In addition to debugging from the Visual Studio IDE, this file was a means to validate that the application was running correctly.  To access the file after it is written you can use FTP.  Out of the box, Windows IoT, runs an ftp server on the Pi and you can connect by navigating to:  ftp://ipaddress in windows explorer.  We used Windows.Storage.ApplicationData.Current.LocalFolder to write the file and this specific file is eventually written to this path:  IPAddress\Users\DefaultAccount\AppData\Local\Packages\{your package}\LocalState\logging.txt (where {your package} seems to be some kind of GUID).

Below are two final pictures to provide an image of the completed project.  Hopefully this information, the pictures, and the code are enough to allow you to reproduce the entire effort quickly.

Here is a picture of the Raspberry Pi in its final configuration.  Notice how simple it is, just a WiFi dongle and the cable attaching it to the breadboard.

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/finalprojectpi.JPG)


And here is the final picture of the breadboard.  Note the orientation of the resistors, the long "leg" (vs. short leg) of the LEDs.  I am a complete newbie with the hardware but after reading a few articles it was fairly easy to setup.  And now that we have come to the end of this "article" (finally) we can move on to bigger and better things.  Controlling a robot that drives around and selects groceries off of shelves, with virtual reality capability, delivered by drone, paying by retinal scan sounds like a beautiful next step.  Anyone care to join?

![RaspberryPi.CSharp](https://github.com/vantivlabs/RaspberryPi.CSharp/blob/master/images/finalprojectbreadboard.JPG)


# Hardware

Not much was mentioned above about the hardware but it should be straightforward to setup the two LEDs, two resistors, and wires following the pictures.  If the pictures are not good enough have a look at the tutorials below in 'Useful Links'.

# Good Things to Know

* There are multiple ways you can connect to the Pi without a monitor.  I tried using these instructions:  https://www.raspberrypi.org/blog/use-your-desktop-or-laptop-screen-and-keyboard-with-your-pi/, but found that all I needed to do was attach the Raspberry Pi directly to my macbook with an ethernet cable and then enable Internet sharing and everything just worked.  Really easy and simple...no monitor needed.

* It is smart to immediately change the default password.

* The USB WiFi dongle that is packaged with the kit works for Raspbian but not for Windows IoT.  If you will be developing for Windows IoT you will need to purchase/bring a WiFi dongle that works with Windows IoT (currently there is only one):  http://ms-iot.github.io/content/en-US/win10/SetupWiFi.htm.  We have tested and the Raspberry Pi WiFi Dongle does indeed work as expected.

* While the case that comes from CanaKit looks cool it does not work with the breadboard.  It turns out this is not correct you can definitely put the Pi in the case with both top and bottom covers on.  The top cover does not completely close due to the grey cable but it still looks cool.  That said, now that the cover is on I think I liked the Pi without the cover.

* Make sure when you plug in the grey cable, which connects the breadboard to the Raspberry Pi, that the red stripe on the cable faces the outside of the Raspberry Pi board.  See picture(s) above for orientation.

* When you plug the t-connector into the breadboard the +/- signs right under (3V3 and 5V) align to row number 1 on the breadboard.

* If you are wondering how far to push the t-connector into the breadboard push it all the way in.  If you tip the breadboard on its side you should not see the silver "legs" of the t-connector.

* If you are going to work with Windows IoT you will need a computer with windows 10 and Visual Studio 2015 Community edition.  Actually, you do not need community edition, any version of Visual Studio 2015 will work.

* If you want to put a different OS version other than Raspbian on the Pi you will need a MicroSD adapter.

* It's likely also a good idea to give your device a unique name.


# Useful Links

https://github.com/InitialState/piot-101/wiki -- great tutorial on getting everything up and running

https://dev.windows.com/en-US/iot -- lots of useful information about windows IoT

# Other cool things

It is amazing how many fantastic things people are building with the Raspberry Pi.  The device epitomizes innovation and getting your hands on one will immediately get your brain churning.

Here's one where a gentleman was creating a cluster with Raspberry Pis but found that creating that cluster was slightly messy...so what did he do?  Created a beautiful looking case!  Check it out!

https://pocketcluster.wordpress.com/2015/07/23/raspberry-pi-2-cluster-case-pt2/

And here's another...

https://learnaddict.com/2015/08/03/raspberry-pi-stack-a-platform-for-learning-about-iot/

# Legal Stuff

[Privacy Policy, Copyright Notices, and Terms of Use](https://vantiv.com/privacy-policy)

Raspberry Pi, Windows, Windows IoT, Visual Studio, and CanaKit are registered or unregistered marks belonging to their respective owners who are unaffiliated with and do not endorse or sponsor Vantiv, and Vantiv likewise does not endorse or sponsor Raspberry Pi, Windows, Windows IoT, Visual Studio, or CanaKit.

