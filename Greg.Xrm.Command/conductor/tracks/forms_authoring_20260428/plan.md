# Implementation Plan: Microsoft Forms Authoring

## Overview
Add PACX-native authoring capabilities for Microsoft Forms source-controlled manifests so the CLI can create, update, delete, publish, and manipulate questions and sections.

## Scope
- Form lifecycle commands for create, update, delete, publish, and archive of the local authoring manifest.
- Question and section manipulation for text, choice, rating, and metadata-driven forms.
- Confirmation safeguards for destructive operations.
- Tests and docs for authoring workflows.

## Improvements
- Extends PACX from read-only Forms operations to full authoring workflows.
- Makes it practical to script form lifecycle management.
- Gives the repo a clean separation between operational exports and authoring.

## Success Criteria
- Users can create and update a form manifest from the CLI.
- Users can add, update, and remove questions and sections.
- Delete/publish actions are safe and documented.
- Tests cover parse and executor behavior for the new surfaces.

## Phases

### Phase 1: Lifecycle commands
- [x] Task: Define the form lifecycle command surfaces.
- [x] Task: Implement create, update, delete, and publish commands.
- [x] Task: Add parse and executor tests.

### Phase 2: Question and section manipulation
- [x] Task: Implement question add/update/delete commands.
- [x] Task: Implement section add/update commands.
- [x] Task: Add tests for ordering, validation, and destructive safeguards.

### Phase 3: Documentation and integration
- [x] Task: Add generated command reference and recipe docs.
- [x] Task: Wire the track into conductor registry and roadmap docs.
- [x] Task: Add usage guidance for permissions and API limitations.

### Phase 4: Inspection and listing
- [x] Task: Implement manifest inspect command.
- [x] Task: Implement question and section list commands.
- [x] Task: Add tests and docs for the read path.

### Phase 5: Section deletion
- [x] Task: Implement section delete command.
- [x] Task: Add delete safeguard tests.
- [x] Task: Update docs and recipe examples.
