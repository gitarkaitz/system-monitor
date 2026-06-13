# System Monitor

A real-time Windows desktop application for monitoring system resources — CPU, RAM, disk, network and active processes — built with WPF and clean architecture principles.

<hr style="height: 1px; border: 0; background-color: rgba(255, 255, 255, 0.15);">

Aplicación de escritorio Windows para monitorización de recursos del sistema en tiempo real — CPU, RAM, disco, red y procesos activos — construida con WPF y principios de arquitectura limpia.

---

## Status / Estado

In active development. Follow the PR history to track progress.

En desarrollo activo. Consulta el historial de PRs para seguir el progreso.

---

## Why this project / Por qué este proyecto

This project serves as a practical bridge from 13 years of C# and Unity3D development into the enterprise .NET ecosystem — WPF, EF Core, SQL, CI/CD tooling.

It was chosen deliberately: real-time resource monitoring maps naturally to the performance-critical, data-intensive work done in VR/AR applications. Same patterns, different platform.

<hr style="height: 1px; border: 0; background-color: rgba(255, 255, 255, 0.15);">

Este proyecto es un puente práctico desde 13 años de desarrollo en C# y Unity3D hacia el ecosistema .NET empresarial — WPF, EF Core, SQL, herramientas CI/CD.

La elección no es casual: la monitorización de recursos en tiempo real conecta directamente con el trabajo de optimización de rendimiento y gestión de datos intensiva realizado en aplicaciones VR/AR. Los mismos patrones, distinta plataforma.

---

## Tech stack / Stack tecnológico

| Layer | Technology |
|---|---|
| UI | WPF + MVVM (CommunityToolkit.Mvvm) |
| Charts | LiveCharts2 |
| Data | Entity Framework Core 8 + SQLite |
| Tests | xUnit + Moq + FluentAssertions |
| CI/CD | GitHub Actions |
| DI | Microsoft.Extensions.DependencyInjection |

---

## Architecture / Arquitectura

Three-layer solution with strict unidirectional dependencies.

Solución en tres capas con dependencias estrictamente unidireccionales.

| SystemMonitor.UI | → | SystemMonitor.Services | → | SystemMonitor.Data |
|---|---|---|---|---|
| | |<center> ↑ </center>| | |
| | | <center> SystemMonitor.Tests  </center>| | |

---

- **UI** — WPF views and ViewModels. No direct data access.
- **Services** — Business logic behind interfaces. No UI knowledge.
- **Data** — EF Core context, models, repositories. No business logic.
- **Tests** — Unit tests with real mocks. No UI dependency.

<hr style="height: 1px; border: 0; background-color: rgba(255, 255, 255, 0.15);">

- **UI** — Vistas y ViewModels WPF. Sin acceso directo a datos.
- **Services** — Lógica de negocio detrás de interfaces. Sin conocimiento de la UI.
- **Data** — Contexto EF Core, modelos y repositorios. Sin lógica de negocio.
- **Tests** — Tests unitarios con mocks reales. Sin dependencia de la UI.

---

## Planned features / Funcionalidades previstas

- [ ] Real-time CPU, RAM, disk and network graphs / Gráficas en tiempo real de CPU, RAM, disco y red
- [ ] Active process list with filtering and kill process / Lista de procesos activos con filtrado y kill process
- [ ] Configurable alerts with system tray notifications / Alertas configurables con notificación en bandeja del sistema
- [ ] Persistent metric history with SQLite / Histórico de métricas persistido en SQLite
- [ ] Fully testable architecture with dependency injection / Arquitectura completamente testable con inyección de dependencias

---

## License / Licencia

MIT — see [LICENSE](LICENSE) for details / consulta [LICENSE](LICENSE) para más detalles.

