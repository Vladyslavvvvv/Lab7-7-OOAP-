using System;
using System.Windows.Forms;

namespace Lab7_OOAP_
{
    public partial class Form1 : Form
    {
        // Поле для зберігання першого елемента ланцюга обробки
        private IHandler _chain;

        // Елементи інтерфейсу
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
            // Створюємо обробників
            var editor = new EditorHandler(lstOutput);
            var layout = new LayoutHandler(lstOutput);
            var designer = new DesignerHandler(lstOutput);

            // Формуємо ланцюг: редактор → макетувальник → дизайнер
            editor.SetNext(layout).SetNext(designer);
            _chain = editor;

            // Додаємо типи задач до комбобокса
            cmbTaskType.Items.AddRange(new[] { "Редагування", "Макетування", "Дизайн" });
            cmbTaskType.SelectedIndex = 0; // Встановлюємо значення за замовчуванням
        }

        // Обробка кліку на кнопку "Додати задачу"
        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var taskType = cmbTaskType.SelectedItem.ToString();   // Отримуємо тип задачі
            var taskDescription = txtTask.Text.Trim();            // Отримуємо опис задачі

            if (!string.IsNullOrWhiteSpace(taskDescription))
            {
                _chain.Handle(taskType, taskDescription); // Запускаємо обробку задачі через ланцюг
                txtTask.Clear();                          // Очищаємо поле вводу
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
            btnAddTask.Click += new EventHandler(btnAddTask_Click); // Прив’язка події натискання

            lstOutput = new ListBox() { Left = 20, Top = 110, Width = 530, Height = 200 };

            this.Controls.Add(lblTask);
            this.Controls.Add(txtTask);
            this.Controls.Add(lblType);
            this.Controls.Add(cmbTaskType);
            this.Controls.Add(btnAddTask);
            this.Controls.Add(lstOutput);
        }
    }

    // Інтерфейс обробника: визначає загальні методи
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);                           // Встановлення наступного обробника
        void Handle(string taskType, string taskDescription);         // Метод обробки задачі
    }

    // Абстрактний базовий клас, реалізує логіку передавання далі
    public abstract class Handler : IHandler
    {
        private IHandler _nextHandler; // Посилання на наступного обробника

        public IHandler SetNext(IHandler handler)
        {
            _nextHandler = handler; // Зв’язування наступного обробника
            return handler;
        }

        public virtual void Handle(string taskType, string taskDescription)
        {
            _nextHandler?.Handle(taskType, taskDescription); // Якщо поточний не обробляє — передає далі
        }
    }

    // Конкретний обробник: редактор
    public class EditorHandler : Handler
    {
        private readonly ListBox _output;

        public EditorHandler(ListBox output) => _output = output;

        public override void Handle(string taskType, string taskDescription)
        {
            if (taskType == "Редагування")
                _output.Items.Add($"Редактор обробив задачу: {taskDescription}");
            else
                base.Handle(taskType, taskDescription); // Якщо не його — передає далі
        }
    }

    // Конкретний обробник: макетувальник
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

    // Конкретний обробник: дизайнер
    public class DesignerHandler : Handler
    {
        private readonly ListBox _output;

        public DesignerHandler(ListBox output) => _output = output;

        public override void Handle(string taskType, string taskDescription)
        {
            if (taskType == "Дизайн")
                _output.Items.Add($"Дизайнер обробив задачу: {taskDescription}");
            else
                _output.Items.Add($"Немає обробника для задачі: {taskDescription}"); // Якщо ніхто не зміг обробити
        }
    }
}