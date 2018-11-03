---
title: テスト
uid: about_test
---

Biauiソリューションはテスト駆動開発の準備が整っています。    
単体テストフレームワークに [xUnit.net](https://xunit.github.io/)、WPF GUIテストに [Friendly](https://github.com/Codeer-Software/Friendly) を採用しています。






## 準備

### Visual Studio 2017 標準テスト

1. [テスト]-[テスト設定]-[規定のプロセッサーアーキテクチャー]を`X64`に設定する。
1. テストエクスプローラーの`テストを並列で実行する` を`オン`に設定する。


### ReSharper

1. [ReSharper]-[Unit Tests]-[Unit Tests Sessions] で `Unit Test Sessions` ウィンドウを開く。
1. [Options]-[Platform] を`Automatic` に設定する。


## テスト実行方法

テストエクスプローラーやReSharperで実行する。

もしくは、コンソールで以下のバッチファイルを実行する。
```
> Biaui\source\AllTest.bat
```

