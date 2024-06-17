using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFactRegressCalc.Infrastructure.Commands.Base
{
    public class LambdaCommandAsync : Command
    {
        private readonly Func<object?, Task> _ExecuteAsync;
        private readonly Func<object?, bool>? _CanExecuteAsync;

        private volatile Task? _ExecutingTask;

        /// <summary>Выполнять задачу принудительно в фоновом потоке</summary>
        public bool Background { get; set; }

        public LambdaCommandAsync(Func<object, Task> ExecuteAsync, Func<object, bool> CanExecuteAsync = null)
        {
            _ExecuteAsync = ExecuteAsync ?? throw new ArgumentNullException(nameof(ExecuteAsync));
            _CanExecuteAsync = CanExecuteAsync;
        }
        protected override bool CanExecute(object? parameter) =>
            (_ExecutingTask is null || _ExecutingTask.IsCompleted)
            && (_CanExecuteAsync?.Invoke(parameter) ?? true);

        protected override async void Execute(object? parameter)
        {
            var background = Background;

            var can_execute = background
                ? await Task.Run(() => CanExecute(parameter))
                : CanExecute(parameter);
            if (!can_execute) return;

            var execute_async = background ?
                Task.Run(() => _ExecuteAsync(parameter)) :
                _ExecuteAsync(parameter);
            _ = Interlocked.Exchange(ref _ExecutingTask, execute_async);
            _ExecutingTask = execute_async;
            OnCanExecuteChanged();

            try
            {
                await execute_async.ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {

            }
            OnCanExecuteChanged();
        }
    }
}
