# Registrador de Tarefas com Categorias

Aplicativo .NET MAUI para gerenciamento de tarefas organizadas por categorias, com build automatizado via GitHub Actions para Android.

## Funcionalidades

O aplicativo permite adicionar, visualizar, concluir e remover tarefas, organizando-as por categorias personalizáveis com cores e ícones. A interface segue o padrão MVVM (Model-View-ViewModel) utilizando o CommunityToolkit.Mvvm para Data Binding e Commands.

## Estrutura do Projeto

O projeto está organizado da seguinte forma:

| Diretório/Arquivo | Descrição |
|---|---|
| `TodoApp/Models/` | Classes de modelo de dados (`TaskItem`, `Category`) |
| `TodoApp/ViewModels/` | Lógica de negócio (`MainViewModel`) com MVVM |
| `TodoApp/Converters/` | Conversores de valor para Data Binding na UI |
| `TodoApp/Platforms/Android/` | Configurações específicas para Android |
| `TodoApp/Resources/` | Ícones, imagens, fontes e estilos |
| `.github/workflows/` | Workflow do GitHub Actions para build do APK |

## Como Baixar o APK

1. Acesse a aba **Actions** deste repositório no GitHub.
2. Clique no workflow **Build Android APK** mais recente.
3. Na seção **Artifacts**, clique em **TodoApp-Android-APK** para baixar o arquivo `.apk`.
4. Transfira o arquivo para o seu dispositivo Android e instale-o (pode ser necessário habilitar "Fontes desconhecidas" nas configurações do dispositivo).

## Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| .NET SDK | 8.0 | Framework de desenvolvimento |
| .NET MAUI | 8.0 | Framework multiplataforma |
| CommunityToolkit.Mvvm | 8.2.2 | Implementação do padrão MVVM |
| GitHub Actions | - | CI/CD para build automatizado |
