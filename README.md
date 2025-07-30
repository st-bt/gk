# First pass

## 1. Observations

### Model / Domain

- Functionality is to register a speaker - single public method
"Speaker" model
- First name, Last name and Email cannot be null or empty (based on validation)
- Experience
  - An integer scale (perhaps years?) from 0 upwards (unbounded) where a lower number represents inexperience
  - Unclear naming on both param and property - consider `ExperienceInYears` or `YearsOfExperience`
  - `Exp` property is nullable - would the domain want this? Ideally we'd check with domain experts.


### Request validation

- Must specify at least 1 session
  - All sessions must be approved (see below notes on approval rules)
- Speaker must meet a standard wherein they must meet at least one of the criterion listed below. However, a speaker that doesn't meet the standard can be permitted if their email address domain is not explicitly flagged and their browser is anything but Internet Explorer Version 8 or below.
    - More than "10" experience
    - Has a blog
    - Has at least 3 certifications
    - Is employed by Pluralsight, Microsoft or Google (as determined by their Employer property)

### Request behaviour
- A proposed session is not approved if it's title or description contains any "old technology" (as defined by ot array: `[Cobol, Punch Cards, Commodore, VBScript]`)
- Fee calculation - "More experienced speakers pay a lower fee". Rules are:
  ```
  < 1 == £500
  [2,3] == £250
  [4,5] == £100
  [6-9] == £50
  Otherwise £0
  ```

### Style
- Unecessary inline comments that explain what the code is technically doing (e.g. "put list of employers in array") as doesn't add any context or reasoning that couldn't be deduced from the syntax and flow.
- Commented out code (rely on source control instead) 
- List<T> where ICollection<T> or Array would suffice since we're not using any specific List<T> behaviour.
- Excessive conditional nesting / identation
- Validation checks are verbose
  - Conditions are negated and have superfluous parenthesis which make it difficult to comprehend at a glance
  - Fee calculation conditions could have their ranges expressed more clearly

#### Parameters
  - Hungarian notation on parameters (and is inconsistently applied)
  - Inconsistent casing on parameters (e.g. _E_mail)
  - Unused parameters (`iExp` (set on model), `strFirstName`, `strLastName`, `BHasBlog`, `URL`, `strBrowser`, `csvCertifications`, `s_emp`, `iFee`, `csvSess`)
  - Some parameters are not descriptive (e.g. `iExp` - ?? Experience? - This is unused so can't infer it's usage)
 
### Behaviour
- Public setter on `Session.Approved` - not bad per se but we might want to be explicit about the behaviour with a domain method

- Shadowing - e.g. `Email` param has the same name as the `Speaker.Email` property which is also in scope. The parameter takes precedence. This makes comprehension difficult and could be the source of bugs in later refactoring it's very easy to think you're referring to the Email property if you forget about the parameter.

- `Speaker.Register` has multiple responsibilities
  - Speaker mixes a model with persistence - not necessarily bad (e.g. Active Record) but generally frowned upon (Single responsibility)
  - Fee calculation - this could be a component and tested in isolation

- `Speaker.Register` mixes params with state of the instance (e.g. Property `Exp` is used but there's an unused param called `iExp`) which is bad for comprehension

- Casting `speakerId` to `int` in the `RegisterResponse` constructor

- Case sensitive checks on strings on things that should probably be insensitive - e.g. when approving sessions and looking in description text for specific phrases

- Swallowed exception when calling `SaveSpeaker` - let the exception bubble up instead

- Bug on line #142 where the variable being used to determine whether a session is approved is not set to false during the loop check. Either set the variable in the loop to false when necessary or use an aggregation over Sessions like `Sessions.Any(s => !s.Approved)`.


## 2. Make it build & write tests to assert our understanding of the domain

Do the least amount necessary to make the code compile ideally without touching the code we're planning to refactor.
Having compiling code and tests will serve as the bedrock for our refactoring to ensure we haven't broken the external behaviour.

### Making it build

- Missing `WebBrowser`, `Session`, `RegisterError`, `IRepository` & `RegisterResponse`

 #### `RegisterResponse`
- Two ctors to allow either Success with speaker ID or failure with RegisterError
	- We can make this nicer in next refactoring step having an `Ok()` / `Fail()` factory method which more obvious to the caller

### Test structure

Use a simple fake test double for the IRepository. I generally prefer this to bringing in a mocking framework unless it's really needed. The test double will provide us a means of asserting some action was taken with the dependency that roughly aligns with the real-world dependency i.e., persistence.

For the validation scenarios that require a greater range / matrix of inputs for a good amount of coverage, I have chosen to use xUnit `MemberData` and express the matrix of values in, what I believe, is a more readable & comprehensible format.

## 3. Next steps

- [x] Fix approval bug which will mean all tests are passing

- [ ] Separate model, registration fee and persistence concerns
  - [ ] Use a request object that encapsulates the modifyable values for the model
  - [ ] Validate the request model in isolation
    - [x] Reduce conditional nesting
    - [x] Use more concise `string.IsNullOrEmpty` methods

- [ ] Registration Fee
  - [ ] Decouple into reusable component and unit test. Probably doesn't need to be abstracted at this point since there's only one obvious implementation so a static instance will suffice.

- [x] Remove try/catch around the repository `SaveSpeaker()` method call
