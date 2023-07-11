﻿using SchoolSchedule.Commands.Base;
using System;

namespace SchoolSchedule.Commands
{
    internal class RelayCommand : Command
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object,bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter!) ?? true;
       
        public override void Execute(object? parameter) => _execute(parameter!);
        
    }
}
