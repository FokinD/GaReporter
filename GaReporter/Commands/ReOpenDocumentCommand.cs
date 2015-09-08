using System;

namespace GaReporter
{
    public class ReOpenDocumentCommand : Tools.CommandExtension<ReOpenDocumentCommand>
    {
        public override void Execute(object parameter)
        {
            //если документ не открылся, то всё сбросить
            try
            {
                ((DocumentView)parameter).SetData(Tools.JsonIO.Open<Document>(GaReporter.Properties.Settings.Default.fileName));
            }
            catch (Exception)
            {

                ((DocumentView)parameter).Clear();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is DocumentView;
        }
    }


}