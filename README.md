# BroDoYouEvenStack 

[![Build status](https://ci.appveyor.com/api/projects/status/m9mn0gnmqa42ge3k?svg=true)](https://ci.appveyor.com/project/patchandthat/brodoyouevenstack) 

[![forthebadge](http://forthebadge.com/images/badges/uses-badges.svg)](http://forthebadge.com) [![forthebadge](http://forthebadge.com/images/badges/made-with-crayons.svg)](http://forthebadge.com)

**Bro do you even stack?** is a tool to remind you when it is time to stack neutral creep camps and warn you of upcoming rune spawns.

## Installing
You can download the latest version [here.](https://ci.appveyor.com/api/projects/patchandthat/brodoyouevenstack/artifacts/Bootstrapper/bin/Release/BroDoYouEvenStackInstaller.exe) You obviously need to have Dota2 installed.

## Changing the port
Port 4000 is used by default to listen for dota's game state. If you need to use a different port, edit the config files in:

1. *\<Dota2 install folder\>*\Game\Dota\Cfg\gamestate_integration\gamestate_integration_stack.cfg
2. C:\Users\\*\<username\>*\AppData\Roaming\BroDoYouEvenStack\bdyes.config *this file gets created after you've run the app and changed some settings.*

## Issues & contributions
Issues and feature requests, [feel free](https://github.com/patchandthat/BroDoYouEvenStack/issues). Or do it yourself and send me a pull request.

## Will I get VAC banned for this?
Nope.  That config file makes Dota publish the data that this app uses.  There's nothing shady going on.
