# Grafana Configuration - Catalog Service

Configuracao de observabilidade para o servico de Catalog usando Grafana, Loki e Jaeger.

## Estrutura

```
grafana/
├── README.md
└── provisioning/
    ├── datasources/
    │   └── datasources.yml      Configuracao Loki + Jaeger
    └── dashboards/
        ├── dashboards.yml        Provisionamento automatico
        └── catalog-api-dashboard.json   Dashboard do Catalog API
```

## Dashboard: Catalog API - Game Management Service

Dashboard customizado para monitoramento do servico de Catalog.

### Paineis Incluidos

1. **Total Games Created** - Contador total de jogos criados no periodo
2. **Total Searches** - Contador total de buscas realizadas
3. **Total Errors** - Contador total de erros (Error + Fatal)
4. **Log Levels Distribution** - Distribuicao de logs por nivel (Info, Warning, Error)
5. **Catalog Activity Over Time** - Serie temporal de criacao de jogos vs buscas
6. **Catalog API Logs (Real-time)** - Logs em tempo real do container
7. **Logs Over Time by Level** - Serie temporal de logs agrupados por nivel
8. **Error Logs** - Logs filtrados apenas por erros
9. **Game Management Logs** - Logs de operacoes de gerenciamento de jogos
10. **Search Activity Logs** - Logs de atividade de busca

### Queries LogQL Utilizadas

#### Logs de Erro
```logql
{container="fcg-catalog-api"} | json | level=~"Error|Fatal" | __error__=""
```

#### Logs de Gerenciamento de Jogos
```logql
{container="fcg-catalog-api"} |~ "(?i)(create|update|delete|activate|deactivate|price)" | json
```

#### Logs de Busca
```logql
{container="fcg-catalog-api"} |~ "(?i)search" | json
```

#### Contadores de Atividade
```logql
# Jogos Criados
count_over_time({container="fcg-catalog-api"} |~ "(?i)game.*created" | json | __error__="" [$__range])

# Buscas Realizadas
count_over_time({container="fcg-catalog-api"} |~ "(?i)search" | json | __error__="" [$__range])
```

## Como Usar

### 1. Iniciar os Servicos
```bash
docker compose up -d
```

### 2. Acessar Grafana
- URL: http://localhost:3000
- Usuario: `admin`
- Senha: `admin`

### 3. Dashboard Pre-configurado
O dashboard e provisionado automaticamente em:
- **Pasta**: FCG
- **Nome**: Catalog API - Game Management Service

### 4. Explorar Logs
Use o Explore para queries ad-hoc:
```logql
{container="fcg-catalog-api"} | json
```

### 5. Visualizar Traces
- Acesse o painel de traces
- Selecione datasource: Jaeger
- Traces automaticamente correlacionados com logs

## Datasources Configurados

### Loki (Default)
- URL: http://loki:3100
- Tipo: Loki
- Uso: Logs estruturados da aplicacao

### Jaeger
- URL: http://jaeger:16686
- Tipo: Jaeger
- Uso: Distributed tracing
- Integracao: Correlacao automatica com logs Loki

## Personalizacao

Os arquivos de provisionamento podem ser editados:
- `datasources.yml` - Adicionar novos datasources
- `dashboards.yml` - Configurar pastas e providers
- `catalog-api-dashboard.json` - Customizar paineis e queries

Mudancas sao aplicadas automaticamente (updateIntervalSeconds: 10).

## Troubleshooting

### Dashboard nao aparece
- Verifique se o Grafana esta rodando: `docker compose ps`
- Verifique logs: `docker compose logs grafana`
- Verifique montagem de volumes no docker-compose.yml

### Logs nao aparecem
- Verifique se o Loki esta rodando: `docker compose ps loki`
- Verifique se os logs estao sendo enviados: `docker compose logs catalog-api`
- Teste query no Explore: `{container="fcg-catalog-api"}`

### Traces nao aparecem
- Verifique se o Jaeger esta rodando: `docker compose ps jaeger`
- Acesse Jaeger UI diretamente: http://localhost:16686
- Verifique se OpenTelemetry esta configurado no ServiceDefaults
