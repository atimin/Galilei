The Galile is the XPCA server written in C# for access to process controll systems in REST style.

Initial idea is descripted in this [document](http://www.xpca.org/rest-pca/)
This project in stage of developing. **Don't use it for real applications.**


Build the project
=======================

Project are writing with using MonoDevelop and may be build in Windows and Linux systems with .NET 2 or Mono 2 frameworks.
For users Mono in Linux systems you must install compiler for C# 2.0:

	sudo apt-get install mono-gmcs 

Getting started
========================

Galilei provide to access to data and to control itself configuration by REST protocol in JSON and XML formats. By default It works on `http://127.0.0.1:3001/'.
For getting information about server use:

	GET http://127.0.0.1:3001/
	GET http://127.0.0.1:3001/.xml

Main types of Galilie is  Node, Point, Engine.

**Node** is a base element of server. All objects are nodes. Server provide CURD model for it:

	GET http://127.0.0.1:3001/node # Read data
	POST http://127.0.0.1:3001/node # Create node or update
	PUT http://127.0.0.1:3001/node #update node
	DELETE http://127.0.0.1:3001/node #delete node
	
**Engine** provides a periodical updating data for points that are described on it.

**Point** provides data and includes model of it calculation. 

Example :

	POST http://127.0.0.1:3001/tank_001
	# create simulator
	POST http://127.0.0.1:3001/tank_001/simulator "type=Simulator"
	# create points for level. Param `engine` consites reference to engine in format `xpca:/path/to/engine`
	POST http://127.0.0.1:3001/tank_001/level_001 "type=RandomPoint&engine=xpca://tank_001/simulator"
	POST http://127.0.0.1:3001/tank_001/level_002 "type=RandomPoint&engine=xpca://tank_001/simulator"
	
	GET http://127.0.0.1:3001/tank_001/level_002
	#or
	GET http://127.0.0.1:3001/tank_001/level_002.xml
	
	
References
=======================

https://github.com/flipback/Galilei - homepage

http://xpca.org - site of XPCA community 

http://groups.google.com/group/xpca - XPCA google group
