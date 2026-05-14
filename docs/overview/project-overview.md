# Project Overview — وصال (Wessal)

## Vision

Wessal is a community-driven social platform that directly connects individuals in need with volunteers willing to give their time and skills. The name **وصال (Wessal)** means "connection" or "union" in Arabic — reflecting the platform's core purpose of bridging the gap between those who need help and those who can provide it.

## Problem Statement

In many communities, people with genuine needs (elderly support, educational help, technical assistance, emergency aid) cannot easily find volunteers, and volunteers cannot easily discover where they are needed. Existing platforms are often:

- Not localized for Arabic-speaking communities
- LTR-first with poor RTL support
- Overly complex or enterprise-focused
- Missing real-time communication between requester and volunteer

Wessal solves this by providing a focused, Arabic-first platform with real-time communication, transparent request tracking, and a gamified incentive system.

## Platform Goals

1. **Reduce friction** between help-seekers and volunteers
2. **Build trust** through ratings, profiles, and transparent request history
3. **Encourage participation** through a points and leveling system
4. **Ensure quality** through an admin moderation layer
5. **Communicate naturally** via embedded real-time chat

## Core Value Proposition

| For Requesters | For Volunteers |
|----------------|----------------|
| Easy request creation with image, category, and schedule | Browse and filter open requests by category and city |
| Real-time tracking of volunteer progress (7 stages) | Accept tasks and earn points upon completion |
| Direct chat with the volunteer | Build a profile with ratings and a volunteer level |
| Rate and review their volunteer experience | Compete on a public leaderboard |

## Platform Identity

- **Language:** Arabic-first, RTL layout
- **Audience:** Arabic-speaking communities (Egypt focus, extendable)
- **Tone:** Warm, community-oriented, trustworthy
- **Design:** Modern glassmorphism with an indigo/emerald palette

## Technology Decision: Why ASP.NET Core MVC?

The team chose ASP.NET Core MVC (not React/Vue SPA) because:

1. **Team familiarity** — All contributors had a C# background
2. **Server-rendered HTML** is simpler for Arabic RTL layouts
3. **Razor views** allow tight integration of server validation with display
4. **SignalR** integrates natively without a separate backend
5. **EF Core** provides rapid schema iteration during active development

> **Trade-off Acknowledged:** The MVC approach creates tighter coupling between the UI and backend. A future API-first architecture (exposing a REST API for a mobile app) would require refactoring the controllers.
