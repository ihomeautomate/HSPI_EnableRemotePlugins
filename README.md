HSPI_EnableRemotePlugins ![HomeSeer Logo](https://lh5.googleusercontent.com/-ouDt6liIFbo/AAAAAAAAAAI/AAAAAAAAAgA/f-7s9hTpPzw/photo.jpg?sz=20)
==========================

This HomeSeer plugin enables remote connections to your HS3-Pi or HomeSeer HomeTroller Zee.

## Installation
* Grab the latest release available on [Bintray](https://bintray.com/ihomeautomate/HomeSeer/HSPI_EnableRemotePlugins) and copy it to `/usr/local/HomeSeer` on your HS3-Pi or HomeTroller Zee
 
        ssh homeseer@<homeseer-server-ip>
        homeseer@HomeTrollerZEE ~ $ wget -O /usr/local/HomeSeer/HSPI_EnableRemotePlugins.dll "http://dl.bintray.com/ihomeautomate/HomeSeer/eu/ihomeautomate/homeseer/HSPI_EnableRemotePlugins/0.0.1/HSPI_EnableRemotePlugins-0.0.1-release.dll"
 
* Restart HomeSeer
        
        homeseer@HomeTrollerZEE ~ $ sudo killall mono
        homeseer@HomeTrollerZEE ~ $ cd /usr/local/HomeSeer
        homeseer@HomeTrollerZEE /usr/local/HomeSeer $ sudo ./go
        
* You should now be able to see `EnableRemotePlugins` showing up in the console output.

        [Startup]->Checking for available plug-ins
        [Plug-In]->Found plug-in: EnableRemotePlugins, version: 0.0.1.35667

* Browse to `http://<homeseer-server-ip>/interfaces` and enable `EnableRemotePlugins`-plugin
* Console output shows `EnableRemotePlugins`-plugin initializing

        [EnableRemotePlugins]->Init plugin
        [EnableRemotePlugins]->Starting timer(s) for remote plugins to be initialized.
        [EnableRemotePlugins]->Initializing eventhandler(s) for remote plugins to be initialized.
        [EnableRemotePlugins]->Remote plug-in API interface started on port 10400
        [Plug-In]->Finished initializing plug-in EnableRemotePlugins

* You should now be able to connect to HomeSeer via a remote-plugin-executable (client)   

        mono HSPI_<remote-plugin-name>.exe server=<homeseer-server-ip>

* Console output shows new incoming connection and initialization (server):

        [EnableRemotePlugins]->Incoming remote connection <client ip:port> (ClientId 1)
        [EnableRemotePlugins]->Incoming remote connection <client ip:port> (ClientId 2)
        [Info]-><Plugin Name>. IP:<client ip:port>
        [EnableRemotePlugins]->Preparing to send .InitIO to 1 plugin(s).
        [EnableRemotePlugins]->'<Plugin Name>:'.InitIO(COM1)
        [EnableRemotePlugins]->'<Plugin Name>:' initialized.                 

## Known issues

Due to version conflicts in the HomeSeer API you're limited to use only free plugins `(HSPI AccessLevel 1)`. 

A possible workaround (untested):
 
* Recompile your plugin against the correct Zee references
* Copy the recompiled .exe to `/usr/local/HomeSeer` on your HS3-Pi or HomeTroller Zee
* Restart HomeSeer
* Reconnect the remote plugin    
* Rejoice?

More info can be found in this [reference thread](http://forums.homeseer.com/showthread.php?t=169287)

## Developers
### Get the source code and compile

The source code for this project is [hosted on github](https://github.com/ihomeautomate/HSPI_EnableRemotePlugins). You can use the regular commandline git tools to get the source code on to your machine.

To get the source code onto your machine:

    git clone https://github.com/ihomeautomate/HSPI_EnableRemotePlugins.git

Build from source:
    
    cd HSPI_EnableRemotePlugins/
    ./gradlew buildAll
    
### Copyright and licensing
    
Code and documentation copyright 2014 [iHomeAutomate](http://www.iHomeAutomate.eu). Code released under the [LGPL-2.1 license](LICENSE.txt). For commercial licensing please contact info[at]ihomeautomate.eu
    
## Social media

<!-- Please don't remove this: Grab your social icons from https://github.com/carlsednaoui/gitsocial -->

[![@ihomeautomate][1.1]][1]
[![iHomeAutomate Facebook page][2.1]][2]
[![+IhomeautomateEu][3.1]][3]
[![github.com/ihomeautomate][6.1]][6]

<!-- links to social media icons -->
<!-- no need to change these -->

<!-- icons with padding -->

[1.1]: http://i.imgur.com/tXSoThF.png (@ihomeautomate)
[2.1]: http://i.imgur.com/P3YfQoD.png (iHomeAutomate facebook page)
[3.1]: http://i.imgur.com/yCsTjba.png (+iHomeAutomateEu)
[4.1]: http://i.imgur.com/YckIOms.png (tumblr icon with padding)
[5.1]: http://i.imgur.com/1AGmwO3.png (dribbble icon with padding)
[6.1]: http://i.imgur.com/0o48UoR.png (github.com/ihomeautomate)

<!-- icons without padding -->

[1.2]: http://i.imgur.com/wWzX9uB.png (twitter icon without padding)
[2.2]: http://i.imgur.com/fep1WsG.png (facebook icon without padding)
[3.2]: http://i.imgur.com/VlgBKQ9.png (google plus icon without padding)
[4.2]: http://i.imgur.com/jDRp47c.png (tumblr icon without padding)
[5.2]: http://i.imgur.com/Vvy3Kru.png (dribbble icon without padding)
[6.2]: http://i.imgur.com/9I6NRUm.png (github icon without padding)


<!-- links to your social media accounts -->
<!-- update these accordingly -->

[1]: http://twitter.com/ihomeautomate
[2]: https://facebook.com/pages/iHomeAutomate/218034961586842
[3]: https://plus.google.com/+IhomeautomateEu
[6]: http://github.com/ihomeautomate

<!-- Please don't remove this: Grab your social icons from https://github.com/carlsednaoui/gitsocial -->    
