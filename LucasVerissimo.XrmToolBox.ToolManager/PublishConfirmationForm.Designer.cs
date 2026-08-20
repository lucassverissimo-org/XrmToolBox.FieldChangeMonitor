#nullable enable

namespace LucasVerissimo.XrmToolBox.ToolManager;

partial class PublishConfirmationForm
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel layout = null!;
    private Label messageLabel = null!;
    private TextBox confirmationTextBox = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button publishButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        layout = new TableLayoutPanel();
        messageLabel = new Label();
        confirmationTextBox = new TextBox();
        buttonsPanel = new FlowLayoutPanel();
        publishButton = new Button();
        cancelButton = new Button();
        layout.SuspendLayout();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(messageLabel, 0, 0);
        layout.Controls.Add(confirmationTextBox, 0, 1);
        layout.Controls.Add(buttonsPanel, 0, 2);
        layout.Dock = DockStyle.Fill;
        layout.RowCount = 3;
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        messageLabel.Dock = DockStyle.Fill;
        messageLabel.Name = "messageLabel";
        confirmationTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        confirmationTextBox.Name = "confirmationTextBox";
        confirmationTextBox.TextChanged += ConfirmationTextBox_TextChanged;
        buttonsPanel.Controls.Add(publishButton);
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Padding = new Padding(0, 9, 0, 0);
        publishButton.AutoSize = true;
        publishButton.DialogResult = DialogResult.OK;
        publishButton.Enabled = false;
        publishButton.Text = "Publicar";
        publishButton.UseVisualStyleBackColor = true;
        cancelButton.AutoSize = true;
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Text = "Cancelar";
        cancelButton.UseVisualStyleBackColor = true;
        AcceptButton = publishButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(534, 201);
        Controls.Add(layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PublishConfirmationForm";
        Padding = new Padding(16);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Confirmar publicação";
        layout.ResumeLayout(false);
        layout.PerformLayout();
        buttonsPanel.ResumeLayout(false);
        buttonsPanel.PerformLayout();
        ResumeLayout(false);
    }
}
