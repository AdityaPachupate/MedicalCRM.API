# Panchakarma CRM: Feature & API Integration Guide

This guide details the application's capabilities across its core modules and the corresponding API endpoints that power them.

---

## 🏗️ Leads Module
*Management of potential patients and their initial contact history.*

### 🌟 What you can do:
1.  **Register New Leads**: Capture patient details and initial interest.
    - `POST /api/leads`
2.  **Browse & Search**: View active leads with paging and filtering. Includes **Calculated Flags** (`HasEnrollment`, `HasMedicine`) for instant status context.
    - `GET /api/leads`
3.  **Deep Dive History**: View a lead's full timeline including follow-ups, enrollments, and **Billing History**.
    - `GET /api/leads/{id}`
4.  **Maintain Accuracy**: Update patient contact info or status as it evolves.
    - `PUT /api/leads/{id}`
    - **Soft Delete (Cascading)**: Move leads to "Trash" while preserving data. This **automatically** trashes all associated Follow-ups, Enrollments, and Bills.
      - `DELETE /api/leads/{id}`
    - **Restore (Cascading)**: Bring a lead and all its associated history back from the trash.
      - `POST /api/leads/{id}/restore`
    - **Permanent Delete**: Completely remove data (Admins only).
      - `DELETE /api/leads/{id}?isPermanent=true`

---

## 📞 Follow-up Module
*Tracking interactions and automating the patient conversion pipeline.*

### 🌟 What you can do:
1.  **Schedule Next Contact**: Assign a follow-up task with a specific priority and communication source (WhatsApp, Call, etc.).
    - `POST /api/followups`
2.  **Daily Action Dashboard**: Instantly see what needs attention today. Includes automatic calculation of **Overdue** tasks.
    - `GET /api/followups/today`
3.  **Record Outcomes**: Complete a task and document the result (e.g., Busy, Interested).
    - `POST /api/followups/{id}/complete`
4.  **Automation (The Workflow Engine)**:
    - **Auto-Rescheduling**: Completing a task as `Busy` automatically creates a new follow-up for the next day.
    - **Lead Updates**: Completing a task as `Not Interested` automatically marks the parent Lead as `Lost`.
5.  **Task Cleanup**:
    - **Soft Delete**: Remove accidental or redundant tasks.
      - `DELETE /api/followups/{id}`
    - **Restore**: Recover a deleted follow-up.
      - `POST /api/followups/{id}/restore`

---

## 📦 Packages Module
*Defining service offerings, durations, and financial baseline.*

### 🌟 What you can do:
1.  **Define Offerings**: Create standardized treatment packages with fixed durations and costs.
    - `POST /api/packages`
2.  **Manage Catalog**: List all active packages available for enrollment.
    - `GET /api/packages`
3.  **Audit Details**: View the specific parameters of a package.
    - `GET /api/packages/{id}`
4.  **Evolution**: Update package prices or durations as business needs change.
    - `PUT /api/packages/{id}`
5.  **Inventory Control**:
    - **Deactivate**: Soft-delete packages that are no longer offered.
      - `DELETE /api/packages/{id}`
    - **Reactivate**: Restore legacy packages.
      - `POST /api/packages/{id}/restore`

---

## 🎓 Enrollments Module
*The bridge between Leads and Packages—converting interest into active participation.*

### 🌟 What you can do:
1.  **Convert a Lead**: Enroll a patient into a package. This **automatically** generates an initial `Bill` and updates the Lead's status to `Converted`.
    - `POST /api/enrollments`
2.  **Track Participation**: View all enrollments with advanced logic:
    - **Real-time Status**: Filter by `isActive=true` to see who is currently in treatment.
    - **Financial Snapshot**: View `PendingAmount` directly in the list.
      - `GET /api/enrollments`
3.  **Detailed Review**: View an enrollment with its **historical snapshots** and its **Unified Financial Status** (Initial Cost, Medicines, Paid vs Pending).
    - `GET /api/enrollments/{id}`
4.  **Adjust Schedules**: Update enrollment dates. Note: The `EndDate` automatically recalculates if you change the `StartDate` or the `Package`.
    - `PUT /api/enrollments/{id}`
5.  **Lifecycle Management**:
    - **Soft Delete**: Trash an enrollment. Note: This unlinks but preserves the associated Bill.
      - `DELETE /api/enrollments/{id}`
    - **Restore**: Recover a trashed enrollment.
      - `POST /api/enrollments/{id}/restore`

---

## 🧾 Bills Module
*Financial tracking and itemized medicine billing.*

### 🌟 What you can do:
1.  **Itemized Billing**: Create bills with multiple medicine items, quantities, and automatic price snapshots.
    - `POST /api/bills`
2.  **Global Financial Overview**: View a paginated list of all bills across the entire clinic. Supports filtering by `LeadId` or `IsTrash`.
    - `GET /api/bills`
3.  **Detailed Lead Billing**: Get the full financial history for a specific lead.
    - `GET /api/leads/{id}/bills`
4.  **Unified Financial Status**: Manage `InitialAmount` (Package), `MedicineBillingAmount`, `AmountPaid`, and `PendingAmount`.
5.  **Historical Integrity**: Soft-deleted bills and inactive medicines remain visible in detail views for audit trails.
6.  **Lifecycle Management**:
    - **Soft Delete**: Move individual bills to the trash.
      - `DELETE /api/bills/{id}`
    - **Update**: Adjust bill totals or itemized medicine lists.
      - `PUT /api/bills/{id}`
    - **Restore**: Recover a trashed bill.
      - `POST /api/bills/{id}/restore`
    - **Permanent Delete**: Hard-delete bills from the system (Admins only).
      - `DELETE /api/bills/{id}?isPermanent=true`

---

## 🚀 Technical Standards & Architecture

This project is built using modern, industry-standard patterns to ensure scalability, maintainability, and high performance.

### 🏗️ Vertical Slice Architecture
Unlike traditional N-Tier architectures, we organize code by **Features** (`Features/Leads/CreateLead`). This encapsulates all requirements for a single functionality (Request, Response, Handler, Validator) in one place, reducing coupling and making the system easier to evolve.

### ⚡ CQRS with MediatR
We use the **Command Query Responsibility Segregation (CQRS)** pattern via MediatR.
- **Commands**: Handle all data modifications (Create, Update, Delete).
- **Queries**: Handle all data retrievals (GetById, List).
This separation allows us to optimize read and write paths independently.

### 🏛️ Repository Pattern & Centralized Logic
We use the **Repository Pattern** (`IBillRepository`) to centralize complex business logic, such as:
- **Medicine Item Processing**: Fetching current prices and creating snapshots.
- **Financial Recalculation**: A unified `RecalculateTotals` method ensures that `PendingAmount` is calculated identically across all modules, preventing data corruption.
- **Dependency Inversion**: Handlers depend on interfaces, making the core business logic database-agnostic and highly testable.

### 🛡️ Pipeline Behaviors & Validation
Input validation is handled through **FluentValidation** and integrated into the MediatR pipeline using **Behaviors**. This ensures that validation logic is decoupled from business logic and runs automatically before any handler is executed.

### 🧐 Observability & Performance
- **Correlation IDs**: Every request is assigned a unique ID via custom middleware, which is logged with **Serilog** for end-to-end traceability in production.
- **Manual Projection**: High-density queries use Mapster's `.ProjectToType<T>()` or manual `.Select()` with `AsNoTracking()` to minimize memory overhead and database latency.
- **Data Integrity (Snapshots)**: Critical historical data (like Package Cost/Duration) is snapshotted during enrollment to protect past transactions from future price changes.

### 🗑️ Lifecycle & Trash System
A consistent **Soft Delete** pattern is implemented across all modules. 
- **Global Filters**: Trashed items are automatically hidden from normal views.
- **Cascading Trash**: Trashing a top-level entity (like a Lead) automatically trashes all dependent records (Follow-ups, Enrollments, Bills) to ensure financial and data consistency.
- **Audit Trails**: Historical detail views (GetById) use `.IgnoreQueryFilters()` to ensure all past data, including trashed records, remains available for accurate reporting.