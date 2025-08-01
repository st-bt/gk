Hi reviewer(s) :wave:

Here's some context that I provided in my original email along with this submission.

I want to try and paint a picture of the approach I took since I'm sure we agree that it's not just the resulting code that is interesting but also the journey to get there.

My submission is attached. There are two projects under a single solution: GK (the program) and GK.Tests (the unit tests). The "Speaker" class can be found in ./GK/Models/Speaker/Speaker.cs.

Just to highlight that I wasn't certain how far you would have liked me to take the refactoring. For that reason I've retained the spirit of the original source code you sent over and applied, what I consider, a generous definition of refactoring since there are some external changes (e.g., method signature changes) that would realistically require changes and recompiliation for external dependents but I just couldn't leave the parameters with Hungarian Notation there! In short, I tried to stay true to what I think you are asking for as opposed to rewriting over refactoring.

My approach was to do 4 rounds of refactoring where a round is generally a set of changes and then I reevaluate to see if I think any further modifications are warranted.

1. - Identify the intended behaviour of the code and write supporting unit tests

It is imperative this happens first for two reasons:

 i.  Our understanding of the domain and outcomes are understood and agreed upon.
 ii. Fast feedback cycle as to whether our refactoring has broken anything.

Of course, to have the tests run I needed to have the code compile and so I also implemented the missing types ("WebBrowser", "Session" etc.) where I provided the absolute minimum functionality to get the tests passing.

I chose to implement the IRepository test double as a fake with a simple implmentation as opposed to using a mocking framework. I do use mocking frameworks but in this case I felt it was overkill but happy to discuss.

2. Decluttering & readability

A round of changes intended to aid the reader's comprehension of the code but have no affect on the system beyond the Speaker class - i.e., no method signature changes, all changes applied within the internals of the Speaker class.

This included:
  - Reducing nesting through restructuring the return sequence (returning early if validation elements fail)
  - Simplify conditional expressions (e.g., using string.IsNullOrEmpty(value) instead of "(value == null) || (value.Length == 0)")
  - Renaming variables to more descriptive names
  - Relocating variable declaration (locality of declaration) to the areas in which they are used. This is intended to aid comprehension since the reader doesn't have to backtrack to consider the scope of the variable across large parts of the code where it is in scope but isn't used.

3. Including external effects

If we're looking at Speaker as the central unit of refactor there are elements here that are not strictly refactoring to some peoples definitions because I alter the signature of the `Speaker.Register` method.

This included:

  External changes:
  - Removing unused parameters (consequently removed shadowing of the Email property)
  - Better encapsulation of the Speaker state. This involved reducing visibility of writable aspects such as public setters with either no setter or init-only. This prevents outside parties reaching into a Speaker instance and potentially corrupting it's state.
  - Improve RegisterResponse type to better model a success/failure flow with static factory methods and a clear bool property indicating whether the register operation was successful.

  Internal changes:
  - Decompose key areas into private methods (e.g., standards checks have their own private method in the Speaker class)

4. Final pass

Any other noticeable improvements and tidy up I thought was practical. This included:

- Turning the "speaker standards" checks into a specification object. This more clearly identified the purpose of those conditionals.
- Organise into namespaces - for readability and future development of the project, it is important to begin better organising the aspects and concerns within the project.


Towards better architecture
================
With a "Clean Architecture" mindset, I would remove the persistence abstraction from the domain model of "Speaker" - generally speaking it's good practice in larger, production-grade projects to have the domain only concerned with domain rules and push the more technical aspects to the outer rings of the architecture.
Having a command processor framework at the Application layer then having a "Register" command that is responsible for interpreting a request model and working the domain layer to turn that request model into a Speaker entity would allow for better separation of the domain and infrastructure concerns of the code. This helps enforce good practices such as Inversion of Control which generally enhances testability and domain understanding through isolating concerns.
