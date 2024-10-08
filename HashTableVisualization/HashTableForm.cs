﻿//!!! Сделать более подробную операцию добавления. Показать, что хэш считается и в его ячейку идет вставка. Совет был ListBox.
namespace HashTableVisualization;

public partial class HashTableForm : Form
{
    private HashTable<int> HashTable;
    private StateManager<int> manager;
    private HashTableVizualizer<int> visualizer;
    public HashTableForm()
    {
        InitializeComponent();
        HashTable = new HashTable<int>();
        manager = new StateManager<int>(HashTable, HashTable.capacity);
        visualizer = new HashTableVizualizer<int>();
        UpdateVisualization();
    }
    //private void ListBoxAddText(int key, int value)
    //{
    //    int hash = Math.Abs(key.GetHashCode() % HashTable.capacity);
    //    ActionsListBox.DrawMode = DrawMode.OwnerDrawVariable;
    //}
    private void btnAdd_Click(object sender, EventArgs e)
    {
        using (var inputForm = new InputForm(true))
        {
            inputForm.DataSubmitted += (key, value) =>
            {
                manager.Insert(key, value);
                ActionsListBox.Items.Insert(0, $"Element with value {value} added in {HashTable.GetHash(key)} Bucket");
                ActionsListBox.Items.Insert(1, $"Hash of key '{key}' calculated: {HashTable.GetHash(key)}");
                btnPreviousStep.Enabled = true;
                UpdateVisualization();
            };
            inputForm.ShowDialog();
        }
    }

    private void btnFind_Click(object sender, EventArgs e)
    {
        using (InputForm inputForm = new InputForm(false))
        {
            inputForm.DataSubmitted += (key, value) =>
            {
                Node<int> result = manager.Find(key);
                MessageBox.Show(result != null ? $"Value found: {result.Value}" : "Value not found", "Find Result");
                UpdateVisualization();
            };
            inputForm.ShowDialog();
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        btnPreviousStep.Enabled = true;
        using (var inputForm = new InputForm(false))
        {
            inputForm.DataSubmitted += (key, value) =>
            {
                if (manager.Remove(key))
                {
                    ActionsListBox.Items.Insert(0, $"Element with key {key} has been removed");
                    UpdateVisualization();
                }
                else
                {
                    MessageBox.Show("Element couldn't be deleted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            inputForm.ShowDialog();
        }
    }

    private void btnNextStep_Click(object sender, EventArgs e)
    {
        //Если текущее состояние - крайнее
        if (manager.GetStateStorage().Next().Equals(manager.GetStateStorage().GetLast()))
            btnNextStep.Enabled = false;
        btnPreviousStep.Enabled = true;
        UpdateVisualization();
    }

    private void btnPreviousStep_Click(object sender, EventArgs e)
    {
        //Если сделав шаг назад, мы попадаем в первое состояние
        if (manager.GetStateStorage().Previous().Equals(manager.GetStateStorage().GetFirst()))
            btnPreviousStep.Enabled = false;
        btnNextStep.Enabled = true;
        UpdateVisualization();
    }
    private void UpdateVisualization()
    {
        HashTableState<int>? currentState = manager.GetStateStorage().GetCurrentState();
        if (currentState == null)
            return;
        Bitmap bitmap = new(pictureBox.Width, pictureBox.Height);
        pictureBox.Image = visualizer.DrawHashTable(currentState, bitmap);
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                manager.SaveStatesToFile(saveFileDialog.FileName);
                MessageBox.Show("Сохранение прошло успешно", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Сохранение не удалось", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void loadToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                manager.LoadStatesFromFile(openFileDialog.FileName);
                manager.UpdateHashTable();
                if (!manager.GetStateStorage().isFirst())
                    btnPreviousStep.Enabled = true;
                UpdateVisualization();
                MessageBox.Show("Загрузка прошла успешно", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Загрузка не удалась", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }

    private void btnInformation_Click(object sender, EventArgs e)
    {
        InfoForm infoForm = new InfoForm();
        infoForm.ShowDialog();
    }
}
