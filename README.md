HSPI_EnableRemotePlugins ![HomeSeer Logo](https://lh5.googleusercontent.com/-ouDt6liIFbo/AAAAAAAAAAI/AAAAAAAAAgA/f-7s9hTpPzw/photo.jpg?sz=20)
==========================

This HomeSeer plugin enables remote connections to your HS3-Pi or HomeSeer HomeTroller Zee.

## Installation
* Grab the latest release available on [Bintray](https://bintray.com/ihomeautomate/HomeSeer/HSPI_EnableRemotePlugins) 
* Copy it to `/usr/local/HomeSeer` on your HS3-Pi or HomeTroller Zee 
* Restart HomeSeer
* You should now be able to connect to HomeSeer via a remote-plugin-executable using `mono HSPI_<remote-plugin-name>.exe server=<homeseer-ip>`


## Known issues

Due to version conflicts in the HomeSeer API you're limited to use only free plugins `(HSPI AccessLevel 1)`. 

A possible workaround (untested):
 
* Recompile your plugin against the correct Zee references
* Copy the recompiled .exe to `/usr/local/HomeSeer` on your HS3-Pi or HomeTroller Zee
* Restart HomeSeer
* Reconnect the remote plugin    
* Rejoice?

More info about remote-plugin licensing quircks can be found in this [reference thread](http://forums.homeseer.com/showthread.php?t=169287)

## Developers
### Get the source code and compile

The source code for this project is [hosted on github](https://github.com/ihomeautomate/HSPI_EnableRemotePlugins). You can use the regular commandline git tools to get the source code on to your machine.

To get the source code onto your machine:

    git clone https://github.com/ihomeautomate/HSPI_EnableRemotePlugins.git

Build from source:
    
    cd HSPI_EnableRemotePlugins/
    ./gradlew buildAll
    
### Copyright and licensing
    
Code and documentation copyright 2014 [iHomeAutomate](http://www.iHomeAutomate.eu). Code released under [the LGPL-2.1 license](LICENSE.txt). For commercial licensing please contact info[at]ihomeautomate.eu
    
## Social media

<!-- Please don't remove this: Grab your social icons from https://github.com/carlsednaoui/gitsocial -->

[![@ihomeautomate][1.1]][1]
[![iHomeAutomate @ Facebook][2.1]][2]
[![+IhomeautomateEu][3.1]][3]
[![github.com/ihomeautomate][6.1]][6]

<!-- links to social media icons -->
<!-- no need to change these -->

<!-- icons with padding -->

[1.1]: http://i.imgur.com/tXSoThF.png (twitter icon with padding)
[2.1]: http://i.imgur.com/P3YfQoD.png (facebook icon with padding)
[3.1]: http://i.imgur.com/yCsTjba.png (google plus icon with padding)
[4.1]: http://i.imgur.com/YckIOms.png (tumblr icon with padding)
[5.1]: http://i.imgur.com/1AGmwO3.png (dribbble icon with padding)
[6.1]: http://i.imgur.com/0o48UoR.png (github icon with padding)

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
