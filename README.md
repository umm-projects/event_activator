# What?

* アプリケーション全体のイベント有効状態を管理するためのクラスを提供します。

# Why?

* 個別のコンポーネント単位で管理していくと状態管理が複雑になってしまうため。

# Install

```shell
$ npm install github:umm-projects/event_activator
```

# Usage

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

```csharp
using UnityEngine;
using UnityEngine.EventSystems;
using UnityModule;

public class Sample : UIBehaviour {
    protected override void Start() {
        base.Start();
        this.SetEventActivation();
    }
}
```

* `UnityEngine.EventSystems.UIBehaviour` を継承しているクラス向けに `SetEventActivation()` という拡張メソッドを提供しています。
* 呼び出すと、 `OnActivateAsObservable()` と `OnDeactivateAsObservable()` を自動的に購読し、同一 GameObject にアタッチされている `UnityEngine.UI.Graphic` の `raycastTarget` のオンオフを切り替える実装を仕込みます。

# License

Copyright (c) 2017 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

