Picagari - The Dependency Injection Library for .Net / Mono
=
## Synopsis

Picagari (Malay for _syringe_) is an Inversion of Control (IoC), Contexts and Dependency Injection (CDI) library for .Net / Mono.
A high level example of what Picagari does would be to allow developers to quickly create classes, without having to create factories for constructing objects, or repeatedly having to manually construct objects, and their dependencies (so on, and so forth). It also helps with the decoupling of interfaces and their implementations.

There are no configurations to write -- just start by adding the `[Inject]` attributes to your fields / properties, and then use `Picagari.Start` on the object that owns them.

## Basic Code Example
With some clever usage of the `[Inject]` attribute on your fields and properties you can:
#### Stop doing this:
```C#
public class Pilot
{
	//# Dependency
    public SpaceShip SpaceShip;
	//# Dependency
    public Uniform Uniform;

    public Pilot()
    {
	    SpaceShip = new SpaceShip();
	    SpaceShip.Engine = new Engine();
	    SpaceShip.Chassis = new Chassis();
	    SpaceShip.Weapon = new Weapon();

		Uniform = new Uniform();
	    Uniform.Shirt = new Shirt();
		//# Etc, etc

	    Uniform.ZipUp();
	    SpaceShip.FlyOut();
    }
}
```

#### And stop doing this:
```C#
public class Pilot
{
	//# Dependency
    public SpaceShip SpaceShip;
	//# Dependency
    public Uniform Uniform;

    public Pilot()
    {
	    SpaceShip = new SpaceShip( new Engine(), new Chassis(), new Weapon() );
	    Uniform = new Uniform( new Shirt() );

	    Uniform.ZipUp();
	    SpaceShip.FlyOut();
    }
}
```

#Now, do _this:_
```C#
public class Pilot
{
	[Inject]
    public SpaceShip SpaceShip;
	[Inject]
    public Uniform Uniform;

    public Pilot()
    {
        //# Construct this class' dependencies (and dependencies' dependencies recursively)
        Picagari.Start( this );
	    Uniform.ZipUp();
	    SpaceShip.FlyOut();
    }
}
```

You can also use `Picagari.Start()` with other objects:
```C#
public class Pilot
{
	//# Dependency
    public SpaceShip SpaceShip;
	[Inject]
    public Uniform Uniform;

    public Pilot()
    {
	    Picagari.Start( this );
	    SpaceShip = Picagari.Start( new SpaceShip() );
	    Uniform.ZipUp();
	    SpaceShip.FlyOut();
    }
}
```
## Attributes

|                       	|                                                                                                                                                                                                                                                                               	|
|-----------------------	|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| `[Inject]`            	| Constructs, the member, and any of the member's `[Inject]` marked dependencies recursively.                                                                                                                                                                                   	|
| `[Produces]`          	| Used on a method, you tell Picagari that you want to use that method for producing the object before you inject it. This is good for when you want to use some logic to decide which implementation of a certain object type you'd like to return (see the [Klingons Rule](https://github.com/XaeroDegreaz/picagari/tree/master/Picagari.Examples/KlingonsRule) example). 	|
| `[Default]`           	| Classes marked with this attribute will be injected by default should there be more than one.implementation of a class, or interface. This ia required on at least one of those classes.                                                                                      	|
| `[ApplicationScoped]` 	| Classes marked with this attribute will only ever be constructed once. They essentially become reusable singletons, that can be injected over, and over but keep the same reference.                                                                                          	|
| (MVC Only) `[SessionScoped]` 	| Similar to ApplicationScoped, classes marked with this attribute will only constructed once, and will be reused throughout a browser session.                                                                                          	|
| (MVC Only) `[RequestScoped]` 	| Classes marked with this scope attribute will only be constructed once, and reused throughout a single request (page load).                                                                                          	|
| `[PostConstruct]` 	| A method marked with this attribute will fire after the instance object is fully constructed, and injected. |

## Saweet! How do I get started?
The usual methods:
* Download the [latest binaries](https://github.com/XaeroDegreaz/picagari/releases), and reference them in your project.
* Checkout the repo, and build from source, or reference the project.
* Read the [Wiki](https://github.com/XaeroDegreaz/picagari/wiki).
* Look at some of the [example classes](https://github.com/XaeroDegreaz/picagari/tree/master/Picagari.Examples).
* Inject away!

## Motivation for the project.

tl;dr - Because IoC dependency injection is completely awesome.

I started working in a pure Java shop that uses Java's CDI framework, and I was fascinated by a number of things:
* How powerful it was.
* How much time was saved.
* How reusable my code became, as I was basically able to compartmentalize & isolate modules of code, and just inject them freely into any object that required them.

I began looking around to see if there were any such frameworks for .Net, the _true_ language of love. I found a few really good ones, namely:
* [Ninject](http://www.ninject.org/)
* [StructureMap](http://structuremap.github.io/structuremap/)

However, my problem with these frameworks is that they rely on some initial configuration, and even more configuration as your project grows. While these frameworks do what they are supposed to do, I felt that they required far too much configuration, and the learning curve was just too steep for something that could be done in a more simple way.

Picagari uses absolutely no configurations of any kind. I've followed the the experience that I've had with Java's CDI framework, and stripped away configuration.

## Tests

There are NUnit tests available in the solution. They test a variety of things from feature functionality, to infinite inject recursion issues (such as injecting the same type into itself...).

## Contribute?

* If you'd like to contribute to the project, simply fork it, hack it, and send a merge request.
* Post bugs / suggestions in the [issue tracker](https://github.com/XaeroDegreaz/picagari/issues).

## License

I'm not a lawyer, so in plain English:

You can use this library in any project, be it personal, commercial or open source.
