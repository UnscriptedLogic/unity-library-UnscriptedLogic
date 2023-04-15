using UnityEngine;

namespace UnscriptedLogic.Builders
{
    public interface IBuildable
    {
        void LocalPassBuildConditions<T>(T builder, out List<LocalBuildCondition> localBuildConditions);
    }

    public interface IBuilder<TBuildable, TBuildableContainer> where TBuildable : IBuildable
    {
        TBuildableContainer[] buildableContainers { get; }

        TBuildable WhenGetBuildable(TBuildableContainer buildableObject);
        void WhenCreateBuildable(int index, Vector3 position, Quaternion rotation, TBuildableContainer buildableContainer);
        void WhenCreatePreview(int index, Vector3 position, Quaternion rotation, TBuildableContainer buildableContainer, out TBuildable buildable, out TBuildableContainer container);
        void OnConditionResult(BuildResult buildResult);
        void ClearPreview(TBuildable buildable, TBuildableContainer container);
    }

    public struct BuildResult
    {
        private bool passed;
        private string description;
        private string failReason;
        private string passReason;

        public bool Passed => passed;
        public string Description => description;
        public string FailReason => failReason;
        public string PassReason => passReason;

        public BuildResult(bool passed, string description, string failReason = "", string passReason = "")
        {
            this.passed = passed;
            this.description = description;
            this.failReason = failReason;
            this.passReason = passReason;
        }
    }

    public class AdminBuildCondition<T>
    {
        private string name;
        private Predicate<T> condition;
        private string onFailConditionText;
        private string onPassConditionText;

        public string Name => name;
        public Predicate<T> Condition => condition;
        public string OnFailConditionText => onFailConditionText;
        public string OnPassConditionText => onPassConditionText;

        public AdminBuildCondition(string name, Predicate<T> condition, string onFailConditionText = "", string onPassConditionText = "")
        {
            this.name = name;
            this.condition = condition;
            this.onFailConditionText = onFailConditionText;
            this.onPassConditionText = onPassConditionText;
        }
    }

    public class LocalBuildCondition
    {
        public delegate bool LocalCondition(Vector3 position, Quaternion rotation);

        private string name;
        private LocalCondition condition;
        private string onFailConditionText;
        private string onPassConditionText;

        public string Name => name;
        public LocalCondition Condition => condition;
        public string FailConditionText => onFailConditionText;
        public string PassConditionText => onPassConditionText;

        public LocalBuildCondition(string name, LocalCondition condition, string onFailConditionText, string onPassConditionText)
        {
            this.name = name;
            this.condition = condition;
            this.onFailConditionText = onFailConditionText;
            this.onPassConditionText = onPassConditionText;
        }
    }

    public class BuildHandlerSimple<TBuildable, Builder, BuildableContainer> where TBuildable : IBuildable where Builder : IBuilder<TBuildable, BuildableContainer>
    {
        public List<AdminBuildCondition<TBuildable>> adminBuildConditions;
        public List<BuildableContainer> buildableObjects;
        private Builder builder;
        private BuildableContainer? previewObject;
        private TBuildable? previewBuildable;

        public Action<int, Vector3, Quaternion, BuildableContainer>? OverrideCreateBuildable;
        public BuildableContainer? PreviewObject => previewObject;
        public TBuildable? PreviewBuildable => previewBuildable;

        public BuildHandlerSimple(Builder builder, List<BuildableContainer> buildableObjects)
        {
            adminBuildConditions = new List<AdminBuildCondition<TBuildable>>();
            this.builder = builder;
            this.buildableObjects = buildableObjects;
        }

        public bool AdminConditionCheck(TBuildable buildable, Action<BuildResult> OnConditionResult)
        {
            for (int i = 0; i < adminBuildConditions.Count; i++)
            {
                if (!adminBuildConditions[i].Condition(buildable))
                {
                    BuildResult failedBuildResult = new BuildResult(false, "Admin Condition " + (i + 1) + ": " + adminBuildConditions[i].Name + " failed.", adminBuildConditions[i].OnFailConditionText, adminBuildConditions[i].OnPassConditionText);
                    OnConditionResult?.Invoke(failedBuildResult);
                    return false;
                }
            }

            BuildResult passedBuildResult = new BuildResult(true, "Admin conditions passed.");
            OnConditionResult?.Invoke(passedBuildResult);
            return true;
        }

        public bool LocalConditionCheck(TBuildable buildable, Vector3 position, Quaternion rotation, Action<BuildResult> OnConditionResult)
        {
            buildable.LocalPassBuildConditions(builder, out List<LocalBuildCondition> localBuildConditions);

            for (int i = 0; i < localBuildConditions.Count; i++)
            {
                if (!localBuildConditions[i].Condition(position, rotation))
                {
                    BuildResult failedBuildResult = new BuildResult(false, "Local Condition " + (i + 1) + ": " + localBuildConditions[i].Name + " failed.", localBuildConditions[i].FailConditionText, localBuildConditions[i].PassConditionText);
                    OnConditionResult?.Invoke(failedBuildResult);
                    return false;
                }
            }

            BuildResult passedBuildResult = new BuildResult(true, "Local conditions passed!");
            OnConditionResult?.Invoke(passedBuildResult);
            return true;
        }

        public void Build(int index, Vector3 position, Quaternion rotation, Action<BuildResult> BuildResult, bool doAdminPass = true, bool doLocalPass = true)
        {
            TBuildable buildable = builder.WhenGetBuildable(buildableObjects[index]);

            if (doAdminPass)
            {
                if (!AdminConditionCheck(buildable, BuildResult))
                {
                    return;
                }
            }

            if (doLocalPass)
            {
                if (!LocalConditionCheck(buildable, position, rotation, BuildResult))
                {
                    return;
                }
            }

            if (OverrideCreateBuildable != null)
            {
                OverrideCreateBuildable(index, position, rotation, buildableObjects[index]);
                OverrideCreateBuildable = null;
                return;
            }

            builder.WhenCreateBuildable(index, position, rotation, buildableObjects[index]);
        }

        public void Preview(int index, Vector3 position, Quaternion rotation)
        {
            builder.WhenCreatePreview(index, position, rotation, buildableObjects[index], out previewBuildable, out previewObject);
        }

        public void ClearPreview()
        {
            builder.ClearPreview(previewBuildable, previewObject);
            previewBuildable = default;
            previewObject = default;
        }

        public void AssessPreviewAdminPass(out BuildResult adminResult)
        {
            BuildResult result = new BuildResult();
            AdminConditionCheck(PreviewBuildable, x => result = x);
            adminResult = result;
        }

        public void AssessPreviewLocalPass(Vector3 position, Quaternion rotation, out BuildResult localResult)
        {
            BuildResult result = new BuildResult();
            LocalConditionCheck(PreviewBuildable, position, rotation, x => result = x);
            localResult = result;
        }
    }
}
