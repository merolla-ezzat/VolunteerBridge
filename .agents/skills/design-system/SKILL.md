---
name: design-system-4040
description: Creates implementation-ready design-system guidance with tokens, component behavior, and accessibility standards. Use when creating or updating UI rules, component specifications, or design-system documentation.
---

<!-- TYPEUI_SH_MANAGED_START -->

# التطوع في جمعية و مستشفي أيادي 4040

## Mission
Deliver implementation-ready design-system guidance for التطوع في جمعية و مستشفي أيادي 4040 that can be applied consistently across dashboard web app interfaces.

## Brand
- Product/brand: التطوع في جمعية و مستشفي أيادي 4040
- URL: https://ayady4040.org/ar/donation/volunteering
- Audience: authenticated users and operators
- Product surface: dashboard web app

## Style Foundations
- Visual style: structured, accessible, implementation-first
- Main font style: `font.family.primary=font-regular`, `font.family.stack=font-regular`, `font.size.base=16px`, `font.weight.base=400`, `font.lineHeight.base=24px`
- Typography scale: `font.size.xs=12.8px`, `font.size.sm=12.96px`, `font.size.md=13.6px`, `font.size.lg=14px`, `font.size.xl=14.4px`, `font.size.2xl=16px`, `font.size.3xl=21.6px`, `font.size.4xl=28px`
- Color palette: `color.text.primary=#212529`, `color.text.secondary=#611b70`, `color.text.tertiary=#777777`, `color.text.inverse=#4d2954`, `color.surface.base=#000000`, `color.surface.muted=#ffffff`, `color.surface.raised=#e6007e`, `color.surface.strong=#f9e3ff`, `color.border.muted=rgb(97, 27, 112) rgb(97, 27, 112) rgb(215, 198, 219)`
- Spacing scale: `space.1=2px`, `space.2=4px`, `space.3=6px`, `space.4=8px`, `space.5=8.53px`, `space.6=10px`, `space.7=12px`, `space.8=15px`
- Radius/shadow/motion tokens: `radius.xs=5px`, `radius.sm=10px` | `motion.duration.instant=100ms`, `motion.duration.fast=150ms`, `motion.duration.normal=200ms`, `motion.duration.slow=300ms`

## Accessibility
- Target: WCAG 2.2 AA
- Keyboard-first interactions required.
- Focus-visible rules required.
- Contrast constraints required.

## Writing Tone
concise, confident, implementation-focused

## Rules: Do
- Use semantic tokens, not raw hex values in component guidance.
- Every component must define required states: default, hover, focus-visible, active, disabled, loading, error.
- Responsive behavior and edge-case handling should be specified for every component family.
- Accessibility acceptance criteria must be testable in implementation.

## Rules: Don't
- Do not allow low-contrast text or hidden focus indicators.
- Do not introduce one-off spacing or typography exceptions.
- Do not use ambiguous labels or non-descriptive actions.

## Guideline Authoring Workflow
1. Restate design intent in one sentence.
2. Define foundations and tokens.
3. Define component anatomy, variants, and interactions.
4. Add accessibility acceptance criteria.
5. Add anti-patterns and migration notes.
6. End with QA checklist.

## Required Output Structure
- Context and goals
- Design tokens and foundations
- Component-level rules (anatomy, variants, states, responsive behavior)
- Accessibility requirements and testable acceptance criteria
- Content and tone standards with examples
- Anti-patterns and prohibited implementations
- QA checklist

## Component Rule Expectations
- Include keyboard, pointer, and touch behavior.
- Include spacing and typography token requirements.
- Include long-content, overflow, and empty-state handling.

## Quality Gates
- Every non-negotiable rule must use "must".
- Every recommendation should use "should".
- Every accessibility rule must be testable in implementation.
- Prefer system consistency over local visual exceptions.

<!-- TYPEUI_SH_MANAGED_END -->
