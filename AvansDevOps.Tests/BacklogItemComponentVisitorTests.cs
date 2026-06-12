using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Interfaces;
using Avans_DevOps.AvansDevOps.Domain.Visitors;
using Xunit;

namespace AvansDevOps.Tests
{
    public class BacklogItemComponentVisitorTests
    {
        private static BacklogItem CreateBacklogItem()
        {
            return new BacklogItem(Guid.NewGuid(), "Backlog item", "Beschrijving", 5);
        }

        private static Activity CreateActivity(string title)
        {
            return new Activity(Guid.NewGuid(), title, "Activity beschrijving");
        }

        private sealed class CountingVisitor : IBacklogWorkItemVisitor
        {
            public int BacklogItemVisitCount { get; private set; }
            public int ActivityVisitCount { get; private set; }

            public void VisitBacklogItem(BacklogItem backlogItem)
            {
                BacklogItemVisitCount++;
            }

            public void VisitActivity(Activity activity)
            {
                ActivityVisitCount++;
            }
        }

        [Fact]
        public void TC_03_BacklogItemComponent_ChildrenContainAddedActivities()
        {
            var backlogItem = CreateBacklogItem();
            var activity1 = CreateActivity("Activity 1");
            var activity2 = CreateActivity("Activity 2");

            backlogItem.AddActivity(activity1);
            backlogItem.AddActivity(activity2);

            Assert.Equal(2, backlogItem.Children.Count);
            Assert.Contains(backlogItem.Children, x => x.Id == activity1.Id);
            Assert.Contains(backlogItem.Children, x => x.Id == activity2.Id);
            Assert.Empty(activity1.Children);
        }

        [Fact]
        public void TC_04_Visitor_AllActivitiesDoneVisitor_ReturnsTrue_WhenAllActivitiesAreDone()
        {
            var backlogItem = CreateBacklogItem();
            var activity1 = CreateActivity("Activity 1");
            var activity2 = CreateActivity("Activity 2");

            backlogItem.AddActivity(activity1);
            backlogItem.AddActivity(activity2);

            activity1.StartWork();
            activity1.MarkDone();
            activity2.StartWork();
            activity2.MarkDone();

            var visitor = new AllActivitiesDoneVisitor();

            backlogItem.Accept(visitor);

            Assert.True(visitor.AllActivitiesDone);
        }

        [Fact]
        public void TC_04_Visitor_AllActivitiesDoneVisitor_ReturnsFalse_WhenAtLeastOneActivityIsNotDone()
        {
            var backlogItem = CreateBacklogItem();
            var doneActivity = CreateActivity("Done activity");
            var todoActivity = CreateActivity("Todo activity");

            backlogItem.AddActivity(doneActivity);
            backlogItem.AddActivity(todoActivity);

            doneActivity.StartWork();
            doneActivity.MarkDone();

            var visitor = new AllActivitiesDoneVisitor();

            backlogItem.Accept(visitor);

            Assert.False(visitor.AllActivitiesDone);
        }

        [Fact]
        public void TC_03_BacklogItemComponent_Accept_VisitsBacklogItemAndAllActivities()
        {
            var backlogItem = CreateBacklogItem();
            backlogItem.AddActivity(CreateActivity("Activity 1"));
            backlogItem.AddActivity(CreateActivity("Activity 2"));

            var visitor = new CountingVisitor();

            backlogItem.Accept(visitor);

            Assert.Equal(1, visitor.BacklogItemVisitCount);
            Assert.Equal(2, visitor.ActivityVisitCount);
        }

        
    }
}