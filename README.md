# カンバン タスク管理 (Windows Forms)

.NET Framework 4.8 を使用した、シンプルで軽量なカンバン形式のタスク管理アプリケーションです。
SDK スタイルのプロジェクトファイルを採用しており、`dotnet CLI` を使用してビルド・実行が可能です。

## 特徴

- **3列のカンバンボード**: TODO、進行中、完了の3つのステータスでタスクを管理。
- **ドラッグ&ドロップ**: タスクカードをマウスで掴んで、直感的に列間を移動できます。
- **データの永続化**: 追加・移動したタスクは、実行ファイルと同じフォルダの `tasks.json` に自動保存されます。
- **レスポンシブデザイン**: ウィンドウサイズに合わせて、タスクカードの幅が自動調整されます。
- **単一ファイル配布**: `Costura.Fody` を使用し、依存関係を全て含んだ単一の EXE ファイルとして配布可能です。

## セットアップとビルド

### 必要な環境
- .NET SDK (ビルド用)
- .NET Framework 4.8 ランタイム (実行用)

### ビルド方法
リポジトリをクローンまたはダウンロードし、プロジェクトのルートディレクトリで以下のコマンドを実行します。

```powershell
# デバッグ出力
dotnet build

# リリース用 (単一EXEの生成)
dotnet publish -c Release
```

### 実行方法
ビルド後、以下のパスにある `MyWinFormsApp.exe` を実行してください。
`bin\Release\net48\win-x86\publish\MyWinFormsApp.exe`

## 技術スタック
- **言語**: C# (最新)
- **フレームワーク**: .NET Framework 4.8
- **UI**: Windows Forms (WinForms)
- **データフォーマット**: JSON (System.Text.Json)
- **ビルドツール**: MSBuild (SDK-Style), Costura.Fody (EXE埋め込み)

## ライセンス
[MIT License](LICENSE)

