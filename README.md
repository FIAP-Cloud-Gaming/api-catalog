# FIAP Cloud Gaming - Catalog Service

Microsservico de catalogo de jogos da FIAP Cloud Gaming. Gerencia cadastro, busca, ativacao e precificacao de jogos na plataforma.

## Arquitetura

Clean Architecture com Domain-Driven Design (DDD) e CQRS:

```
src/
├── Catalog.Domain/           Entidades, Value Objects, Eventos de Dominio
├── Catalog.Infrastructure/   EF Core, PostgreSQL, MongoDB, Redis, Elasticsearch, Repositorios
└── Catalog.API/              Endpoints, Behaviors, Validacoes, Middleware
```

## Funcionalidades

### Catalogo de Jogos
- CRUD completo de jogos (titulo, descricao, preco, generos, plataformas)
- Ativacao e desativacao de jogos (publicar/despublicar)
- Alteracao de preco com historico automatico
- Busca por genero, plataforma e status

### Busca Avancada (Elasticsearch)
- Busca full-text com filtros combinados
- Paginacao de resultados
- Re-indexacao manual de jogos

### Reviews e Avaliacoes
- Sistema de reviews com rating (1-5)
- Compra verificada e horas jogadas
- Estatisticas de avaliacao agregadas

### Eventos de Dominio
- GameCreated, GameUpdated, GameDeleted
- GamePublished, GameUnpublished
- GamePriceChanged
- ReviewAdded

## Endpoints

Todos os endpoints requerem autenticacao JWT.

### Games

| Metodo | Rota | Descricao | Auth |
|--------|------|-----------|------|
| POST | `/api/games` | Criar jogo | Sim |
| GET | `/api/games/{id}` | Buscar por ID | Sim |
| PUT | `/api/games/{id}` | Atualizar jogo | Sim |
| DELETE | `/api/games/{id}` | Deletar jogo | Sim |
| PUT | `/api/games/{id}/price` | Alterar preco | Sim |
| PUT | `/api/games/{id}/activate` | Ativar jogo | Sim |
| PUT | `/api/games/{id}/deactivate` | Desativar jogo | Sim |
| GET | `/api/games/search` | Buscar com filtros | Sim |
| GET | `/api/games/genre/{genre}` | Buscar por genero | Sim |
| GET | `/api/games/platform/{platform}` | Buscar por plataforma | Sim |
| GET | `/api/games/status/{status}` | Buscar por status | Sim |
| POST | `/api/games/reindex` | Re-indexar jogos | Sim |

## Stack

| Componente | Tecnologia |
|------------|------------|
| Framework | .NET 10.0 |
| CQRS | FiapCloudGaming.Mediator |
| DDD | FiapCloudGaming.SeedWork |
| Observabilidade | FiapCloudGaming.ServiceDefaults (OpenTelemetry + Serilog) |
| Banco relacional | PostgreSQL 18 + EF Core |
| Banco documental | MongoDB |
| Cache | Redis |
| Busca | Elasticsearch 9 |
| Validacao | FluentValidation |
| Logging | Serilog + Grafana Loki |
| Tracing | Jaeger (OTLP) |
| Dashboards | Grafana |
| Documentacao API | Scalar (OpenAPI) |

## Executar localmente

### Pre-requisitos
- .NET 10.0 SDK
- Docker Desktop

### Docker Compose
```bash
docker compose up -d
```

### Servicos disponiveis

| Servico | URL |
|---------|-----|
| Catalog API | http://localhost:5002 |
| Grafana | http://localhost:3000 (admin/admin) |
| Jaeger UI | http://localhost:16686 |
| Kibana | http://localhost:5601 |
| PostgreSQL | localhost:5432 |
| MongoDB | localhost:27017 |
| Redis | localhost:6379 |
| Elasticsearch | http://localhost:9200 |

### Executar sem Docker
```bash
dotnet run --project src/Catalog.API
```

## Observabilidade

### Logs
- Logs estruturados com Serilog
- Centralizacao via Grafana Loki
- Formato JSON com contexto completo

### Traces
- Traces distribuidos com OpenTelemetry
- Exportacao para Jaeger via OTLP
- Correlacao automatica entre requisicoes

### Health Checks
- `/health` - Health check geral do servico
- `/alive` - Liveness probe para Kubernetes

### Dashboards
- Dashboard Grafana pre-configurado
- Metricas de operacoes de jogos
- Metricas de buscas
- Rastreamento de erros

## Dependencias (NuGet)

Pacotes compartilhados da solucao:
- `FiapCloudGaming.Mediator` - Implementacao CQRS com MediatR
- `FiapCloudGaming.SeedWork` - Building blocks DDD
- `FiapCloudGaming.ServiceDefaults` - Configuracoes de observabilidade

## Licenca

MIT
