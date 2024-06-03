# Life with AI
## 好きなあの子とどこへでも　あなたとAIで作る物語
　生成AIを用いた自動生成により，あらかじめ用意されたシーンやシナリオが続く従来のゲームとは異なりあなたとAIの対話によって展開が変化していくファンワイズアドベンチャーゲーム(Fanwise Adventure Game)です．
## ファンワイズアドベンチャーゲームとは
　カテゴリ名の由来は，本プロジェクト最大の特徴である自動生成にあります．「fanwise」は「扇形に」を意味する副詞です．[^1]従来のアドベンチャーゲームはフラグによるシナリオ分岐があり，各シナリオは枝に例えられることもあります．それに対し，本プロジェクトは自動生成によってあらゆるシナリオに変化し得るものです．これは前方に向かって広がりをもつ扇形のようなスペクトラムであると考えました．

[^1]: 文法的には「fan-like」というような形容詞のほうが正しいですが，響きを考えてこちらにしました．

## デモプレイ: こんなこともできます！
![demo](asset_for_readme/demo_x4.gif)
　このように，従来のゲームでは不可能だった柔軟な展開を実現できます．なお，こちらのデモは実際の4倍速となっています．
## 備考
- 現在はロジック構築にとどまっており，AIエージェントの立ち絵やペルソナテキストを変更する機能は未だ実装していません．便宜上，「ほのか」というキャラクターが入っています．こちらは卒業研究のときに制作したキャラクターで，立ち絵やペルソナテキストなど本キャラクターに関わる著作権は私に帰属します．  
- シーン生成時に時間がかかり，プレイヤーに待ち時間が発生してしまうのが現時点で最大の課題になっています．  
- .gitignoreにはgithubでリポジトリを作るときに追加できるもののほか，サイズの大きいフォントの入ったフォルダとAPI keyが入ったフォルダ，ロジックに関わるスクリプトが記述されており，それらがアップロードされていません．