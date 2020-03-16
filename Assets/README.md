# EventActivator

* アプリケーション全体のイベント有効状態を管理するためのクラスを提供します。

## Requirement

* [UniRx](https://github.com/neuecc/unirx) ([umm版](https://github.com/umm/unirx))

## Install

### With Unity Package Manager

```bash
upm add package dev.upm-packages.event-activator
```

Note: `upm` command is provided by [this repository](https://github.com/upm-packages/upm-cli).

You can also edit `Packages/manifest.json` directly.

```jsonc
{
  "dependencies": {
    // (snip)
    "dev.upm-packages.event-activator": "[latest version]",
    // (snip)
  },
  "scopedRegistries": [
    {
      "name": "Unofficial Unity Package Manager Registry",
      "url": "https://upm-packages.dev",
      "scopes": [
        "dev.upm-packages"
      ]
    }
  ]
}
```

### Any other else (classical umm style)

```shell
npm install github:umm/event_activator
```

## Usage

### 個別の実装

```csharp
using UnityEngine;
using UnityModule;
using UniRx;

public class Sample : MonoBehaviour {
    public void Start() {
        EventActivator.Instance.OnActivateAsObservable().Subscribe(
            (_) => {
                // 有効になったときの処理
                // コライダの enabled を true にするなど
            }
        )
        EventActivator.Instance.OnDeactivateAsObservable().Subscribe(
            (_) => {
                // 無効になったときの処理
                // コライダの enabled を false にするなど
            }
        )
    }
    public void Sample1() {
        EventActivator.Instance.Deactivate();
    }
    public void Sample2() {
        EventActivator.Instance.Activate();
    }
}
```

* `OnActivateAsObservable()` が返すストリームを購読し、イベントが有効になったときの処理を実装します。
* `OnDeactivateAsObservable()` が返すストリームを購読し、イベントが無効になったときの処理を実装します。
* ボタンクリック時など、排他的なイベント実行を行いたい場合に `Deactivate()` メソッドを呼び出します。
* 次画面に遷移した場合や、次のボタンが表示されるタイミングなどで `Activate()` メソッドを呼び出し、無効状態を解除します。

### 一括設定

```csharp
using UnityEngine;
using UnityModule; // この using がないと Extension Method が設定されません

public class Sample : MonoBehaviour {
    private void Start() {
        this.RegisterEventActivationHandler();
    }
}
```

* `UnityEngine.Component` 向けに `RegisterEventActivationHandler()` という拡張メソッドを提供しています。
* 呼び出すと、 `OnActivateAsObservable()` と `OnDeactivateAsObservable()` を自動的に購読し、同一 GameObject にアタッチされている `UnityEngine.UI.Graphic` の `raycastTarget` や `UnityEngine.Collider`, `UnityEngine.Collider2D` の `enabled` のオンオフを切り替える実装を仕込みます。
* 第一引数に真偽値を渡すと子孫 Component を含むかどうかを変更可能です。
  * default: `true`
  * `false` を渡すと、自身の GameObject のみを対象とします。

### 型の制約

```csharp
using UnityEngine;
using UnityModule; // この using がないと Extension Method が設定されません

public class Sample : MonoBehaviour {
    private void Start() {
        this.RegisterEventActivationHandler<BoxCollider2D>();
    }
}
```

* `UnityEngine.UI.Graphic`, `UnityEngine.Collider`, `UnityEngine.Collider2D` の何れかの継承型に限って有効無効を操作します。
* 「とにかく何らかの 2D Collider を On/Off」という場合には `this.RegisterEventActivationHandler<Collider2D>()` とすると良いでしょう。

## License

Copyright (c) 2017-2018 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)
