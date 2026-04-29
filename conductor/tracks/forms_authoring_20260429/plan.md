# Implementation Plan: Forms Authoring

## Overview
Full Microsoft Forms authoring capability — create, update, delete, publish, inspect forms with question/section manipulation.

## Scope
- ~20 command files covering full forms lifecycle.
- ~28 unit/integration tests.
- Question types: text, choice, rating, date, ranking, Likert, Net Promoter Score.
- Section management: add, remove, reorder.

## Improvements
- Complete forms authoring from the CLI.
- Enables CI/CD for forms-based data collection.

## Success Criteria
- `pacx forms create` — create a new form with questions.
- `pacx forms update` — modify form properties.
- `pacx forms delete` — remove a form.
- `pacx forms publish` — make form available.
- `pacx forms inspect` — show full form definition.
- `pacx forms question add/remove/update/reorder` — manipulate questions.
- `pacx forms section add/remove/reorder` — manipulate sections.
- All commands have unit and integration tests.

## Phases

### Phase 1: Core forms CRUD
- [ ] Task: `pacx forms create` implementation.
- [ ] Task: `pacx forms get` — retrieve form definition.
- [ ] Task: `pacx forms update` implementation.
- [ ] Task: `pacx forms delete` implementation.
- [ ] Task: `pacx forms publish` / `pacx forms unpublish`.
- [ ] Task: Tests for core CRUD operations.

### Phase 2: Question manipulation
- [ ] Task: `pacx forms question add` — add questions with type routing.
- [ ] Task: `pacx forms question remove` — delete a question.
- [ ] Task: `pacx forms question update` — modify question properties.
- [ ] Task: `pacx forms question reorder` — reorder questions.
- [ ] Task: Tests for all question operations.

### Phase 3: Section manipulation
- [ ] Task: `pacx forms section add` — add a section.
- [ ] Task: `pacx forms section remove` — delete a section (with question migration).
- [ ] Task: `pacx forms section reorder` — reorder sections.
- [ ] Task: Tests for section operations.

### Phase 4: Inspect & reporting
- [ ] Task: `pacx forms inspect` — full form definition with metadata.
- [ ] Task: Response summary command.
- [ ] Task: Tests.

### Phase 5: PR Lifecycle
- [ ] Task: Upstream PR; merge.
