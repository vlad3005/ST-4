using System;
using Stateless;

namespace BugPro
{
    public enum State
    {
        New,
        Analysis,
        Fixing,
        Testing,
        CannotReproduceReview,
        Closed,
        InReview,
        Deploying
    }

    public enum Trigger
    {
        Create,
        AssignToFix,
        DeclineNotADefect,
        DeclineWontFix,
        DeclineDuplicate,
        NoTimeNow,
        NeedSeparateSolution,
        OtherProductProblem,
        NeedMoreInfo,
        DeveloperCannotReproduce,
        Fixed,
        TesterConfirmCannotReproduce,
        TesterDenyCannotReproduce,
        TesterConfirmFixed,
        TesterDenyFixed,
        Reopen,
        SendToReview,
        ReviewPassed,
        ReviewFailed,
        DeployAction,
        DeploySuccess,
        DeployFailed
    }

    public class Bug
    {
        private readonly StateMachine<State, Trigger> _machine;

        public State CurrentState => _machine.State;
        
        public Bug()
        {
            _machine = new StateMachine<State, Trigger>(State.New);

            _machine.Configure(State.New)
                .Permit(Trigger.Create, State.Analysis);

            _machine.Configure(State.Analysis)
                .Permit(Trigger.AssignToFix, State.Fixing)
                .Permit(Trigger.DeclineNotADefect, State.Closed)
                .Permit(Trigger.DeclineWontFix, State.Closed)
                .Permit(Trigger.DeclineDuplicate, State.Closed);

            _machine.Configure(State.Fixing)
                .Permit(Trigger.NoTimeNow, State.Analysis)
                .Permit(Trigger.NeedSeparateSolution, State.Analysis)
                .Permit(Trigger.OtherProductProblem, State.Analysis)
                .Permit(Trigger.NeedMoreInfo, State.Analysis)
                .Permit(Trigger.DeveloperCannotReproduce, State.CannotReproduceReview)
                .Permit(Trigger.Fixed, State.InReview); 

            _machine.Configure(State.InReview)
                .Permit(Trigger.ReviewPassed, State.Testing)
                .Permit(Trigger.ReviewFailed, State.Fixing);

            _machine.Configure(State.CannotReproduceReview)
                .Permit(Trigger.TesterConfirmCannotReproduce, State.Closed)
                .Permit(Trigger.TesterDenyCannotReproduce, State.Analysis);

            _machine.Configure(State.Testing)
                .Permit(Trigger.TesterConfirmFixed, State.Deploying) 
                .Permit(Trigger.TesterDenyFixed, State.Analysis);

            _machine.Configure(State.Deploying)
                .Permit(Trigger.DeploySuccess, State.Closed)
                .Permit(Trigger.DeployFailed, State.Fixing);

            _machine.Configure(State.Closed)
                .Permit(Trigger.Reopen, State.Analysis);
        }

        public void Fire(Trigger trigger)
        {
            _machine.Fire(trigger);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Демонстрация WorkFlow работы с багом с использованием Stateless\n");
            var bug = new Bug();
            Console.WriteLine($"Начальное состояние: {bug.CurrentState}");
            
            bug.Fire(Trigger.Create);
            Console.WriteLine($"После создания [Create]: {bug.CurrentState}");
            
            bug.Fire(Trigger.AssignToFix);
            Console.WriteLine($"После взятия в работу [AssignToFix]: {bug.CurrentState}");

            bug.Fire(Trigger.Fixed);
            Console.WriteLine($"После исправления разработчиком [Fixed]: {bug.CurrentState}");

            bug.Fire(Trigger.ReviewPassed);
            Console.WriteLine($"После успешного код ревью [ReviewPassed]: {bug.CurrentState}");

            bug.Fire(Trigger.TesterConfirmFixed);
            Console.WriteLine($"После проверки тестировщиком (ОК) [TesterConfirmFixed]: {bug.CurrentState}");

            bug.Fire(Trigger.DeploySuccess);
            Console.WriteLine($"После успешного внедрения [DeploySuccess]: {bug.CurrentState}");
        }
    }
}
