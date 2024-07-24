# Creative Docs .NET improvement ideas

This is a list of possible improvements to the codebase of Creative Docs .NET.
They are not strictly necessary for the project to work but they would make the code easier to maintain.

## Architecture

### High coupling / Singleton pattern with public static field
There are several classes that implement a singleton pattern by having an instance of the class as a static field. Since those classes often also have a bunch of publicly settable properties, this is isomorphic to having a bunch of global variables.
This is bad for many reasons:
- it makes the code difficult to test: those singletons needs to be properly initialised
- it makes the tests non-deterministic: tests will pass/fail depending in which order they are executed because of some state shared through those singletons. If we really want to keep those singletons, we should at least provide a way to reset them.
- code using the singleton is tightly coupled to the singleton

See also [Coupling (wikipedia)](https://en.wikipedia.org/wiki/Coupling_(computer_programming))

### Wrong dependencies between modules
Some modules depend on each other in a strange way. For instance, some classes that are clearly part of the business logic of the application have dependencies on the UI, which makes it impossible to use them in isolation.

### Implicit assumptions
Many methods have implicit assumptions about their parameters or their surrounding context.
They do not check for theses assumptions, which lead to crashes later down the chain.

For instance, imagine a method that takes an object of type `Foo` and expects it to have a non-null attribute `bar`. That method should check its parameter before doing anything else, and through an error if the parameter is not correct. Otherwise, we get a NullReferenceException way later, debugging takes a lot more time. In several cases, the error will not even occur during this method call. In such cases, the method call responsible for the error is not in the stack trace, which takes even longer to debug.

### static constructors and Initialize()
Several classes have a static constructor that initializes some state.
The runtime calls these static constructors when needed, this is not user controlled.
However, there are many Initialize() methods (most of which are empty) that are called to trigger the static constructors in a semi-deterministic way. This makes debugging harder, as the debugger does not always manages to step into those static constructors

### Duplication
There are several classes or methods that seem to do similar things or have similar code.


## Overengineering

### Complex / hidden control flow
There are different mechanisms that increase the difficulty of following the control flow:
- C# events: when firing an event, it's difficult to know which handlers might have been registered and in which order they will be executed
- callback queues: when adding a callback to such a queue, one does not know exactly when it will get executed

### Wrong asynchronous code
Managing the synchronization between multiple threads or processes is not easy.
Asynchronous code can quickly introduce data races or deadlocks.
Use threads only when strictly necessary.
I removed a lot of asynchronous code that had incorrect synchronization and was adding unnecessary complexity.

### Caching
Caches add complexity to a codebase, it is difficult to invalidate them properly and they can create data duplication. If you need to implement one, follow the single responsibility principle and make the caching behavior an individual component.


## Maintainability

### Handling of paths
Paths are currently handled manually with strings and concatenations.
It would probably be better to use a standard library for that.

### Magic numbers
There are quite a few hard-coded values here and there in the codebase.
For instance, many paths to resources are hard-coded.

### Classes with same name
There are several classes and files with the same name in different namespaces.
It would probably be clearer to rename them.


## Testing

### Non-deterministic test results
Many tests will pass or fail at random, depending in which order they are executed.
This is a direct consequence of the architectural issues of the codebase.

### Lack of tests
There is a general lack of tests. To a large extent, many classes are currently not properly testable anyway because of the great coupling between many classes. 

### Tests without assert
There are many tests that do not explicitly check anything (no assert). They only ensure that the code runs.

### Manual grouping of tests
Some tests are run together through a RunTests method. They should be run separately by the test runner.
I refactored most of those, there is still one such instance in Common.Tests.Drawing.OpenTypeTest

### Async
A lot of asynchronous code is poorly tested with manual waiting loops of Thread.Sleep to wait for asynchronous callback completion.
