using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace UnityModule {

    public class EventActivator : Singleton<EventActivator> {

        /// <summary>
        /// 有効状態を管理する Subject
        /// </summary>
        private readonly BehaviorSubject<bool> subjectActivation = new BehaviorSubject<bool>(true);

        /// <summary>
        /// 有効化する
        /// </summary>
        public void Activate() {
            this.subjectActivation.OnNext(true);
        }

        /// <summary>
        /// 無効化する
        /// </summary>
        public void Deactivate() {
            this.subjectActivation.OnNext(false);
        }

        /// <summary>
        /// Activate 時に発火するストリームを返す
        /// </summary>
        /// <returns>ストリーム</returns>
        public IObservable<Unit> OnActivateAsObservable() {
            return this.subjectActivation.Where(x => x).AsUnitObservable();
        }

        /// <summary>
        /// Deactivate 時に発火するストリームを返す
        /// </summary>
        /// <returns>ストリーム</returns>
        public IObservable<Unit> OnDeactivateAsObservable() {
            return this.subjectActivation.Where(x => !x).AsUnitObservable();
        }

        /// <summary>
        /// 有効か無効かを返す
        /// </summary>
        /// <returns>有効か無効かの状態</returns>
        public bool IsActivated() {
            return this.subjectActivation.Value;
        }

    }

    public static class ComponentExtension {

        [System.Obsolete("Please use Component.RegisterEventActivationHandler() instead of this extension method.")]
        public static void SetEventActivation(this UIBehaviour self) {
            self.RegisterEventActivationHandler();
        }

        /// <summary>
        /// UnityEngine.Component にイベントの有効/無効処理を自動で仕込みます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        public static void RegisterEventActivationHandler(this Component self, bool includeChildren = true) {
            EventActivator.Instance.OnActivateAsObservable().Subscribe(_ => self.HandleEventActiovation(true, includeChildren)).AddTo(self);
            EventActivator.Instance.OnDeactivateAsObservable().Subscribe(_ => self.HandleEventActiovation(false, includeChildren)).AddTo(self);
        }

        /// <summary>
        /// UnityEngine.Component にイベントの有効/無効処理を自動で仕込みます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        /// <typeparam name="T">操作対象の Graphic/Collider/Collider2D の型</typeparam>
        public static void RegisterEventActivationHandler<T>(this Component self, bool includeChildren = true) where T : Component {
            EventActivator.Instance.OnActivateAsObservable().Subscribe(_ => self.HandleEventActiovation<T>(true, includeChildren)).AddTo(self);
            EventActivator.Instance.OnDeactivateAsObservable().Subscribe(_ => self.HandleEventActiovation<T>(false, includeChildren)).AddTo(self);
        }

        /// <summary>
        /// UnityEngine.Component のイベント有効無効状態を切り替えます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="activation">有効無効の状態</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        public static void HandleEventActiovation(this Component self, bool activation, bool includeChildren = true) {
            self.HandleGraphicEventActiovation<Graphic>(activation, includeChildren);
            self.HandleColliderEventActiovation<Collider>(activation, includeChildren);
            self.HandleCollider2DEventActiovation<Collider2D>(activation, includeChildren);
        }

        /// <summary>
        /// UnityEngine.Component のイベント有効無効状態を切り替えます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="activation">有効無効の状態</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        /// <typeparam name="T">操作対象の Graphic/Collider/Collider2D の型</typeparam>
        public static void HandleEventActiovation<T>(this Component self, bool activation, bool includeChildren = true) where T : Component {
            if (typeof(T).IsSubclassOf(typeof(Graphic))) {
                self.HandleGraphicEventActiovation<T>(activation, includeChildren);
            }
            if (typeof(T).IsSubclassOf(typeof(Collider))) {
                self.HandleColliderEventActiovation<T>(activation, includeChildren);
            }
            if (typeof(T).IsSubclassOf(typeof(Collider2D))) {
                self.HandleCollider2DEventActiovation<T>(activation, includeChildren);
            }
        }

        /// <summary>
        /// UnityEngine.Component の Graphic 系クラスに関するイベント有効無効状態を切り替えます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="activation">有効無効の状態</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        /// <typeparam name="T">操作対象の Graphic/Collider/Collider2D の型</typeparam>
        public static void HandleGraphicEventActiovation<T>(this Component self, bool activation, bool includeChildren = true) where T : Component {
            if (includeChildren) {
                self.gameObject.GetComponentsInChildren<T>().Cast<Graphic>().ToList().ForEach(x => x.raycastTarget = activation);
            } else {
                self.gameObject.GetComponents<T>().Cast<Graphic>().ToList().ForEach(x => x.raycastTarget = activation);
            }
        }

        /// <summary>
        /// UnityEngine.Component の Collider 系クラスに関するイベント有効無効状態を切り替えます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="activation">有効無効の状態</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        /// <typeparam name="T">操作対象の Graphic/Collider/Collider2D の型</typeparam>
        public static void HandleColliderEventActiovation<T>(this Component self, bool activation, bool includeChildren = true) where T : Component {
            if (includeChildren) {
                self.gameObject.GetComponentsInChildren<T>().Cast<Collider>().ToList().ForEach(x => x.enabled = activation);
            } else {
                self.gameObject.GetComponents<T>().Cast<Collider>().ToList().ForEach(x => x.enabled = activation);
            }
        }

        /// <summary>
        /// UnityEngine.Component の Collider2D 系クラスに関するイベント有効無効状態を切り替えます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="activation">有効無効の状態</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        /// <typeparam name="T">操作対象の Graphic/Collider/Collider2D の型</typeparam>
        public static void HandleCollider2DEventActiovation<T>(this Component self, bool activation, bool includeChildren = true) where T : Component {
            if (includeChildren) {
                self.gameObject.GetComponentsInChildren<T>().Cast<Collider2D>().ToList().ForEach(x => x.enabled = activation);
            } else {
                self.gameObject.GetComponents<T>().Cast<Collider2D>().ToList().ForEach(x => x.enabled = activation);
            }
        }

    }

}
