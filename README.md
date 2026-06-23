# Деканат колледжа (вариант 7)

WinForms + .NET 8 + EF Core + SQLite

## Особенности UI
- **Синий** акцент (#2563EB)
- **Горизонтальная** навигация сверху (не боковая панель)
- Списки в виде **строк** (ListRow), не карточек

## Запуск
```bash
dotnet run --project Deanery.Forms
```

## Структура
- `Deanery.Data` — Student, Discipline, Grade
- `Deanery.Forms` — формы и UI
- `Deanery.Tests` — тесты GradeCalculator
