# AiStil.TddWithCopilot

This repository is a small exercise created for a meetup session: **TDD pairing with GitHub Copilot**.
The goal is not to build a full product:

- Start from a scenario.
- Write a failing test (first rule of TDD) and help Copilot learn your style ([commit fa68dda](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/fa68dda4706eec002ca411c3ac6d100b19c6d81c)).
- Implement the smallest change to make it pass.
- (Early on) Add new types in the same file as the test class, to keep the step-by-step flow easy to follow ([commit 3b8fbae](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/3b8fbae3edff37185676307b92269cd361330b0b)).
- Refactor (when safe), let Copilot refactor based on your prompt.
- Challenge Copilot to explain what was implemented, stay curious.
- Now you can ask Copilot to implement the next scenario, test + production code.
- Ask Copilot about architecture decision, naming, project structure.
- At some point Copilot can start suggesting next refactoring steps.
- When you need to access an external layer, add a contract at that boundary and use fakes in tests ([1d96ffd](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/1d96ffd50ff705a3fe4e09908cd8fec754d51fb1)).

## How to read this repo

Each step was committed incrementally. Commit messages include the prompt used at that step, so you can follow the conversation and the evolution of the code. Expand the commit message description.

If you just want the highlights, jump directly to these key commits:

1. Write a failing test first: [fa68dda](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/fa68dda4706eec002ca411c3ac6d100b19c6d81c)
2. Add early production code in the same file: [3b8fbae](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/3b8fbae3edff37185676307b92269cd361330b0b)
3. Add a repository contract + use a Fake in tests: [1d96ffd](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/1d96ffd50ff705a3fe4e09908cd8fec754d51fb1)
4. Refactor into a business domain (`Stylist`, `Slot`): [f47e35e](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/f47e35e19a25a96bee42f89cf21f0238bc5daf97)

## How far to refactor?

One important step in the flow is introducing **contracts** (interfaces) at the boundary to other layers.
For example, when the use case needs to read/write scheduling data, add a repository interface (a contract) and keep the business rules independent from the persistence details.
This gives us the freedom to evolve the business implementation in isolation, and in tests we can swap the real implementation with a simple Fake that behaves deterministically.

How far you go with refactoring and design depends on the context and goals of the project.
It can be perfectly fine to stop early (for example, keep booking logic in the use case) if that keeps the code simpler and still readable.
As the domain grows, moving responsibilities into domain types (for example `Stylist` and `Slot`) can keep business rules explicit.[f47e35e](https://github.com/flaviu-boldea/AiStil.TddWithCopilot/commit/f47e35e19a25a96bee42f89cf21f0238bc5daf97)

One refactoring idea we used a lot is **Tell, Don't Ask**: prefer telling objects what you want to achieve (behavior) instead of asking for data and making decisions somewhere else. In practice, this often means moving logic from the use case into domain methods, which also helps with encapsulation (the object keeps control over its own invariants).

## Scenarios

### Successfully booking an appointment for an available slot

- Given there is an available slot starting at `2024-07-01T10:00`
- When the client books an appointment for that slot
- Then the appointment should be successfully created
- And the slot should be marked as booked

### Rejecting booking for a busy slot

- Given an existing busy slot at `2024-07-01T10:30`
- When the client books an 1h appointment for a slot starting at `2024-07-01T10:00` that overlaps existing slot
- Then the booking should be rejected

## Solution structure (current)

- `AiStil.App/Domain`: domain model (business rules). This layer should not depend on repository abstractions.
- `AiStil.App/UseCases`: application use cases that orchestrate the domain and persistence.
- `AiStil.Tests`: tests driving the implementation.

## Tests: sociable vs solitary

This point led to a useful discussion at the meetup: **sociable** vs **solitary** unit tests.

- **Sociable tests** exercise multiple collaborators together (fewer/more coarse-grained tests). They tend to be more resilient to refactoring because they assert behavior across a slice of the system.
- **Solitary tests** isolate the unit test using test doubles/mocks for collaborators. They often make failures easier to pinpoint and can guide design at a very small scale.

In this exercise, many tests are intentionally more sociable (they drive behavior through the use case with a fake repository and real domain types) rather than having a test for every class.

As the domain becomes clearer and more stable, it can make sense to add more direct tests against domain types (for example, testing `Stylist.Book(...)` rules directly). The balance depends on what you optimize for: refactoring flexibility, fast feedback, and how quickly you want to localize failures.

## Run tests

From the repo root:

```bash
dotnet test
```

## Credits

- Mark Seemann: inspired me to create this repo following the “commit-by-commit” approach (*Code That Fits In Your Head*), so someone can follow the changes step by step.
- Zoran Horvat: inspired me to practice **Tell, Don't Ask** (from the course https://www.udemy.com/course/beginning-oop-with-csharp/).
