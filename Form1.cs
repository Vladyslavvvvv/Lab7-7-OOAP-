using System;
using System.Windows.Forms;

namespace Lab7_OOAP_
{
    public partial class Form1 : Form
    {
        private IHandler _chain;

        private TextBox txtTask;
        private ComboBox cmbTaskType;
        private Button btnAddTask;
        private ListBox lstOutput;
        private Label lblTask;
        private Label lblType;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUI();
            InitializeChain();
        }

        private void InitializeChain()
        {
            var editor = new EditorHandler(lstOutput);
            var layout = new LayoutHandler(lstOutput);
            var designer = new DesignerHandler(lstOutput);

            editor.SetNext(layout).SetNext(designer);
            _chain = editor;

            cmbTaskType.Items.AddRange(new[] { "Редагування", "Макетування", "Дизайн" });
            cmbTaskType.SelectedIndex = 0;
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var taskType = cmbTaskType.SelectedItem.ToString();
            var taskDescription = txtTask.Text.Trim();

            if (!string.IsNullOrWhiteSpace(taskDescription))
            {
                _chain.Handle(taskType, taskDescription);
                txtTask.Clear();
            }
            else
            {
                MessageBox.Show("Введіть опис задачі.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeCustomUI()
        {
            this.Text = "Редакція журналу - Chain of Responsibility";
            this.Size = new System.Drawing.Size(600, 400);

            lblTask = new Label() { Text = "Опис задачі:", Left = 20, Top = 20, Width = 100 };
            txtTask = new TextBox() { Left = 130, Top = 20, Width = 400 };

            lblType = new Label() { Text = "Тип задачі:", Left = 20, Top = 60, Width = 100 };
            cmbTaskType = new ComboBox() { Left = 130, Top = 60, Width = 200 };

            btnAddTask = new Button() { Text = "Додати задачу", Left = 350, Top = 60, Width = 180 };
            btnAddTask.Click += new EventHandler(btnAddTask_Click);

            lstOutput = new ListBox() { Left = 20, Top = 110, Width = 530, Height = 200 };

            this.Controls.Add(lblTask);
            this.Controls.Add(txtTask);
            this.Controls.Add(lblType);
            this.Controls.Add(cmbTaskType);
            this.Controls.Add(btnAddTask);
            this.Controls.Add(lstOutput);
        }
    }

    public interface IHandler
    {
        IHandler SetNext(IHandler handler);
        void Handle(string taskType, string taskDescription);
    }

    public abstract class Handler : IHandler
    {
        private IHandler _nextHandler;

        public IHandler SetNext(IHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual void Handle(string taskType, string taskDescription)
        {
            _nextHandler?.Handle(taskType, taskDescription);
        }
    }

    public class EditorHandler : Handler
    {
        private readonly ListBox _output;

        public EditorHandler(ListBox output) => _output = output;

        public override void Handle(string taskType, string taskDescription)
        {
            if (taskType == "Редагування")
                _output.Items.Add($"Редактор обробив задачу: {taskDescription}");
            else
                base.Handle(taskType, taskDescription);
        }
    }

    public class LayoutHandler : Handler
    {
        private readonly ListBox _output;

        public LayoutHandler(ListBox output) => _output = output;

        public override void Handle(string taskType, string taskDescription)
        {
            if (taskType == "Макетування")
                _output.Items.Add($"Макетувальник обробив задачу: {taskDescription}");
            else
                base.Handle(taskType, taskDescription);
        }
    }

    public class DesignerHandler : Handler
    {
        private readonly ListBox _output;

        public DesignerHandler(ListBox output) => _output = output;

        public override void Handle(string taskType, string taskDescription)
        {
            if (taskType == "Дизайн")
                _output.Items.Add($"Дизайнер обробив задачу: {taskDescription}");
            else
                _output.Items.Add($"Немає обробника для задачі: {taskDescription}");
        }
    }
}