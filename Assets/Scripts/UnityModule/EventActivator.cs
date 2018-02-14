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

    }

    public static class ComponentExtension {

        [Obsolete("Please use Component.RegisterEventActivationHandler() instead of this extension method.")]
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
        /// UnityEngine.Component のイベント有効無効状態を切り替えます
        /// </summary>
        /// <param name="self">Component のインスタンス</param>
        /// <param name="activation">有効無効の状態</param>
        /// <param name="includeChildren">子孫 Component の有効無効状態を設定するかどうか</param>
        public static void HandleEventActiovation(this Component self, bool activation, bool includeChildren = true) {
            if (includeChildren) {
                self.gameObject.GetComponentsInChildren<Graphic>().ToList().ForEach(x => x.raycastTarget = activation);
                self.gameObject.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.enabled = activation);
                self.gameObject.GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = activation);
            } else {
                self.gameObject.GetComponents<Graphic>().ToList().ForEach(x => x.raycastTarget = activation);
                self.gameObject.GetComponents<Collider>().ToList().ForEach(x => x.enabled = activation);
                self.gameObject.GetComponents<Collider2D>().ToList().ForEach(x => x.enabled = activation);
            }
        }

    }

}