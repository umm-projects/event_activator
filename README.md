# EventActivator

* アプリケーション全体のイベント有効状態を管理するためのクラスを提供します。

## Requirement

* [UniRx](https://github.com/neuecc/unirx) ([umm版](https://github.com/umm-projects/unirx))

## Install

```shell
npm install github:umm-projects/event_activator
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

## License

Copyright (c) 2017-2018 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

