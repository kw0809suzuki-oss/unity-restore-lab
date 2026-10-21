# Unity Restore Lab

Unity アバター改変の「バックアップがある」と「実際に復元できる」を分けて検査する、独立した公開実験場です。第三者アセットや個人のプロジェクトをアップロードしないでください。

## 現在地（2026-10-21）

- GitHub Actions **run #1**: preflight **成功**、Unity EditMode **SKIPPED**。
- 実行結果: https://github.com/kw0809suzuki-oss/unity-restore-lab/actions/runs/39525514209
- Unity Editorを起動した実証、および4個のEditModeテスト合格は **未確認**。
- スキップはGitHub Secretsに `UNITY_LICENSE` / `UNITY_SERIAL` が未設定のため。正規のライセンスが必要です。
- この状態でActions全体が緑色でも、復元テストが成功したことにはなりません。

## 構成

- `UnityProject/ProjectSettings/ProjectVersion.txt`: Unity 2022.3.22f1
- `UnityProject/Packages/manifest.json`: Unity Test Framework
- `UnityProject/Assets/Tests/Editor/RestoreProbeTests.cs`: 合成Material／Prefab Variantの4試験
- `.github/workflows/unity-editmode.yml`: 事前検査とGameCI Unityテスト（手動再実行可）

## 実機検証へ進む方法

### PCでの実行（Unity Personalの場合はこれが確実）

1. Unity HubでUnity Personalライセンスを認証し、Unity 2022.3.22f1をインストールする。
2. この公開リポジトリを取得し、Unity Hubから `UnityProject` を開く。
3. Unity Editorの **Window > General > Test Runner** で **EditMode** を選び、試験を実行する。
4. コンパイルエラー、テスト実行ログ、4件の合否を記録する。結果を出すまでは「未検証」のままとする。

### GitHub Actionsでの実行

1. 使用可能な正規Unityライセンスを確認する。Unity Personalの手動.ulf認証は公式仕様で非対応。Personalの場合はUnity Hubでの認証を基本とする。
2. 利用可能なGameCI対応ライセンスがある場合のみ、GitHubの Settings > Secrets and variables > Actions で必要な値を設定する。**ライセンスファイルや認証情報をコミットしない。**
3. Actions > Unity Restore Lab > Run workflow で実行し、**unity-editmode** ジョブがSKIPPEDではなく実行されたことを確認する。
4. EditMode結果を保存し、失敗すればコード／環境／ライセンスの問題を切り分ける。

現在のCI設定は `UNITY_LICENSE` または `UNITY_SERIAL` の存在を確認する。秘密情報の設定有無は取得していないし、実際のライセンス認証の成功も未確認。

## 現段階の4検証

- MaterialのGUIDと実体の存在確認
- GUIDが同一でもMaterial内容が変更し得ること
- 完全なMaterial／.metaの復元
- Prefab VariantのTransform Override保存

これらは合成素材を対象にした小規模なEditModeテストであり、VRChat SDK、実アバターの衣装、BlendShape、Animationの復元検証は含まない。

## 判断境界

事前検査成功 ≠ Unity Editorの起動成功 ≠ 復元テスト成功 ≠ 実際のVRChatアバター復元成功 ≠ 商品需要の確認。

Unity公式ライセンス案内: https://docs.unity.com/en-us/engine/6000.0/manual/get-started/install-and-upgrade/licenses-and-activation/license-activation-methods
GameCI: https://game.ci/docs/github/test-runner/
