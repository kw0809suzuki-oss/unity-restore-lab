# Unity Restore Lab

Unity の復元が実際に成立するかを検査する独立した研究用リポジトリです。

**現状:** CI設定とEditMode試験コードを設置中。Unity Editorでの合格はまだ確認していません。

- 対象: 人工的に作るUnityアセット。実際の購入済みアバターや第三者素材は扱いません。
- 目的: 参照の成立と、アセット内容・設定の保存を分けて確かめる。
- 基準Unity: 2022.3.22f1。
- UnityProject/Assets/Tests/Editor/: Unity Test Framework用の試験コード。
- .github/workflows/unity-editmode.yml: 手動実行のGitHub Actions。
- CI実行にはUnityの正規ライセンス設定が必要です。秘密情報はファイルやActionsログへ記載しないでください。

## 実験の境界

GUIDやアセットが存在するだけで正常復元と判定しません。
試験が通ってもVRChat SDK上の実アバター復元まで検証したことにはなりません。
前提と結果を分けて報告します。
