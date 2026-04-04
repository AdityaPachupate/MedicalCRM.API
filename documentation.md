# Panchakarma CRM: Feature & API Integration Guide

This guide details the application's capabilities across its core modules and the corresponding API endpoints that power them.

---

## 🏗️ Leads Module
*Management of potential patients and their initial contact history.*

### 🌟 What you can do:
1.  **Register New Leads**: Capture patient details and initial interest.
    - `POST /api/leads`
2.  **Browse & Search**: View active leads with paging and filtering.
    - `GET /api/leads`
3.  **Deep Dive History**: View a lead's full timeline including all past follow-ups and current enrollments.
    - `GET /api/leads/{id}`
4.  **Maintain Accuracy**: Update patient contact info or status as it evolves.
    - `PUT /api/leads/{id}`
5.  **Lifecycle Management (Trash System)**:
    - **Soft Delete**: Move leads to "Trash" while preserving data.
      - `DELETE /api/leads/{id}`
    - **Restore**: Bring a lead back from the trash.
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
1.  **Convert a Lead**: Enroll a patient into a package. This **automatically** updates the Lead's status to `Converted`.
    - `POST /api/enrollments`
2.  **Track Participation**: View all enrollments with advanced logic:
    - **Real-time Status**: Filter by `isActive=true` to see who is currently in treatment.
    - **Performance Reports**: Filter by `createdAt` range for sales stats or `startDate` range for operational planning.
      - `GET /api/enrollments`
3.  **Detailed Review**: View an enrollment with its **historical snapshots** (Cost/Duration) to see exactly what the patient agreed to, even if the package has since changed.
    - `GET /api/enrollments/{id}`
4.  **Adjust Schedules**: Update enrollment dates. Note: The `EndDate` automatically recalculates if you change the `StartDate` or the `Package`.
    - `PUT /api/enrollments/{id}`
5.  **Lifecycle Management**:
    - **Soft Delete**: Trash an enrollment.
      - `DELETE /api/enrollments/{id}`
    - **Restore**: Recover a trashed enrollment.
      - `POST /api/enrollments/{id}/restore`

---

## 🚀 Technical Standards & Optimization
- **Performance**: High-density queries use Mapster `.ProjectToType<T>()` and `AsNoTracking()` for minimal latency.
- **Robustness**: Global `BusinessException` handling ensures meaningful error messages across the entire API.
- **Integrity**: Snapshot patterns protect historical financial data from future changes.
 logs, and system performance.