using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public static class UIBehaviourExtension {

        /// <summary>
        /// UnityEngine.EventSystems.UIBehaviour にイベントの有効/無効処理を自動で仕込みます
        /// </summary>
        /// <param name="self">UIBehaviour のインスタンス</param>
        public static void SetEventActivation(this UIBehaviour self) {
            EventActivator.Instance.OnActivateAsObservable().Subscribe(_ => self.GetComponent<Graphic>().raycastTarget = true).AddTo(self);
            EventActivator.Instance.OnDeactivateAsObservable().Subscribe(_ => self.GetComponent<Graphic>().raycastTarget = false).AddTo(self);
        }

    }

}