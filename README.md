# Render Bug Reproduction Repo

Simple repo that re-creates a render bug introduced in Avalonia 11.3.1 that makes the TreeDataGrid flicker
when large amounts of items are present and a large amount of them are updated often.

## Change the total number of items in the table
https://github.com/giard-alexandre/Avalonia.RenderBugRepro/blob/8090397a51916d4c20fcfb53abfecd32b4a0fb04/RenderBugRepro/Services/PersonService.cs#L78

## Change number of updates per tick
https://github.com/giard-alexandre/Avalonia.RenderBugRepro/blob/8090397a51916d4c20fcfb53abfecd32b4a0fb04/RenderBugRepro/Services/PersonService.cs#L100
