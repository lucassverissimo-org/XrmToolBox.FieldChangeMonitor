using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    internal partial class ReviewForm : Form
    {
        private readonly Dictionary<MappingRow, DataGridViewRow> mappingRows =
            new Dictionary<MappingRow, DataGridViewRow>();
        private Func<IProgress<MappingProgress>, Task<List<OperationResult>>> saveOperation;
        private Func<IWin32Window, Task> solutionOperation;
        private bool running;
        private int animationFrame;
        public bool HasExecuted { get; private set; }
        public List<OperationResult> Results { get; private set; } = new List<OperationResult>();

        public ReviewForm()
        {
            InitializeComponent();
        }

        public ReviewForm(IEnumerable<MappingRow> changes)
            : this()
        {
            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }
            var rows = changes.ToList();
            summary.Text = string.Join(
                "   |   ",
                new[] { "Removido", "Alterado", "Adicionado" }.Select(kind =>
                    "Mapeamentos "
                    + kind.ToLowerInvariant()
                    + "s: "
                    + rows.Count(row => row.Change == kind)
                )
            );
            foreach (var row in rows.OrderBy(row => row.Change))
            {
                var index = changesGrid.Rows.Add(
                    row.Change,
                    row.OriginalSource,
                    row.OriginalTarget,
                    row.Source,
                    row.Target,
                    "Aguardando confirmação",
                    "Aguardando",
                    string.Empty
                );
                mappingRows.Add(row, changesGrid.Rows[index]);
            }
        }

        public ReviewForm(
            IEnumerable<MappingRow> changes,
            Func<IProgress<MappingProgress>, Task<List<OperationResult>>> operation,
            Func<IWin32Window, Task> addToSolution = null
        )
            : this(changes)
        {
            saveOperation = operation ?? throw new ArgumentNullException(nameof(operation));
            solutionOperation = addToSolution;
        }

        public ReviewForm(string title, IEnumerable<OperationResult> results)
            : this()
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }
            Text = title;
            Results = results.ToList();
            HasExecuted = true;
            summary.Text = "Resultado por operação";
            notice.Visible = false;
            addToSolution.Visible = false;
            foreach (var name in new[] { "OriginalSource", "OriginalTarget", "Source", "Target" })
            {
                changesGrid.Columns[name].Visible = false;
            }
            changesGrid.Columns["Action"].HeaderText = "Operação";
            changesGrid.Columns["Action"].FillWeight = 220F;
            foreach (var result in Results)
            {
                AddResultRow(result);
            }
            SetCompletedState();
        }

        private async void ConfirmClick(object sender, EventArgs eventArguments)
        {
            if (HasExecuted)
            {
                DialogResult = DialogResult.OK;
                return;
            }
            if (running || saveOperation == null)
            {
                return;
            }
            running = true;
            confirm.Enabled = false;
            cancel.Enabled = false;
            executionStatus.Text = "Validando ajustes...";
            animationTimer.Start();
            var progress = new Progress<MappingProgress>(UpdateProgress);
            try
            {
                Results = await saveOperation(progress);
                foreach (var result in Results)
                {
                    if (result.Mapping == null)
                    {
                        AddResultRow(result);
                    }
                    else
                    {
                        UpdateProgress(
                            new MappingProgress
                            {
                                Mapping = result.Mapping,
                                State = result.State ?? (result.Success ? "Salvo" : "Falha"),
                                Detail = result.Detail,
                            }
                        );
                    }
                }
            }
            catch (Exception exception)
            {
                Results = new List<OperationResult>
                {
                    new OperationResult
                    {
                        Operation = "Validação / execução",
                        State = "Falha",
                        Detail = exception.Message,
                    },
                };
                foreach (var mapping in mappingRows.Keys)
                {
                    UpdateProgress(
                        new MappingProgress
                        {
                            Mapping = mapping,
                            State = "Não confirmado",
                            Detail =
                                exception.Message
                                + " Revise o estado no servidor antes de tentar novamente.",
                        }
                    );
                }
                AddResultRow(Results[0]);
            }
            finally
            {
                running = false;
                HasExecuted = true;
                animationTimer.Stop();
                SetCompletedState();
            }
        }

        private void SetCompletedState()
        {
            confirm.Text = "Fechar";
            confirm.Enabled = true;
            addToSolution.Enabled =
                solutionOperation != null
                && Results.Any(result => result.Mapping != null && result.Success)
                && Results.Any(result => result.State == "Publicado" && result.Success);
            cancel.Visible = false;
            CancelButton = confirm;
            executionStatus.Text =
                "Concluído: "
                + Results.Count(result => result.Success)
                + " operação(ões) com sucesso; "
                + Results.Count(result => result.IsWarning)
                + " aviso(s) / não confirmado(s); "
                + Results.Count(result => !result.Success && !result.IsWarning)
                + " falha(s). Consulte os detalhes no grid.";
            if (mappingRows.Count > 0)
            {
                notice.Text =
                    "A gravação e a publicação têm resultados separados. Ajustes que falharam continuam disponíveis na tela principal para correção e nova tentativa.";
            }
            changesGrid.Invalidate();
        }

        private async void AddToSolutionClick(object sender, EventArgs eventArguments)
        {
            if (running || !addToSolution.Enabled || solutionOperation == null)
            {
                return;
            }
            running = true;
            addToSolution.Enabled = false;
            confirm.Enabled = false;
            executionStatus.Text = "Preparando inclusão na solution...";
            try
            {
                await solutionOperation(this);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this,
                    exception.Message,
                    "Adicionar à solution",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                running = false;
                SetCompletedState();
            }
        }

        private void AddResultRow(OperationResult result)
        {
            var state = result.State ?? (result.Success ? "Sucesso" : "Falha");
            var index = changesGrid.Rows.Add(
                result.Operation,
                null,
                null,
                null,
                null,
                state,
                state,
                result.Detail
            );
            ApplyState(changesGrid.Rows[index], state, result.Detail);
        }

        private void UpdateProgress(MappingProgress progress)
        {
            if (IsDisposed)
            {
                return;
            }
            if (progress.Mapping == null)
            {
                executionStatus.Text = progress.Detail;
                return;
            }
            if (mappingRows.TryGetValue(progress.Mapping, out var row))
            {
                ApplyState(row, progress.State, progress.Detail);
                executionStatus.Text =
                    progress.State
                    + ": "
                    + row.Cells["Source"].Value
                    + " → "
                    + row.Cells["Target"].Value;
            }
        }

        private void ApplyState(DataGridViewRow row, string state, string detail)
        {
            row.Cells["State"].Value = state;
            row.Cells["Progress"].Value = state;
            row.Cells["Detail"].Value = detail;
            row.Cells["Detail"].ToolTipText = detail;
            var color = Color.White;
            if (
                state == "Salvo"
                || state == "Publicado"
                || state == "Sucesso"
                || state == "Verificado no pacote"
            )
            {
                color = Color.Honeydew;
            }
            else if (state == "Em execução")
            {
                color = Color.AliceBlue;
            }
            else if (state == "Falha")
            {
                color = Color.MistyRose;
            }
            else if (state == "Resultado incerto" || OperationResult.IsWarningState(state))
            {
                color = Color.LemonChiffon;
            }
            row.DefaultCellStyle.BackColor = color;
            row.DefaultCellStyle.SelectionBackColor = color;
            row.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void AnimationTimerTick(object sender, EventArgs eventArguments)
        {
            animationFrame = (animationFrame + 7) % 100;
            changesGrid.InvalidateColumn(changesGrid.Columns["Progress"].Index);
        }

        private void ChangesGridCellPainting(
            object sender,
            DataGridViewCellPaintingEventArgs eventArguments
        )
        {
            if (
                eventArguments.RowIndex < 0
                || eventArguments.ColumnIndex != changesGrid.Columns["Progress"].Index
            )
            {
                return;
            }
            eventArguments.PaintBackground(eventArguments.ClipBounds, false);
            var bounds = Rectangle.Inflate(eventArguments.CellBounds, -5, -7);
            var state = Convert.ToString(eventArguments.Value);
            var track = Rectangle.Intersect(bounds, eventArguments.ClipBounds);
            if (track.Width > 0 && track.Height > 0)
            {
                eventArguments.Graphics.FillRectangle(Brushes.Gainsboro, track);
                var graphicsState = eventArguments.Graphics.Save();
                eventArguments.Graphics.SetClip(track);
                if (state == "Em execução")
                {
                    var width = Math.Max(20, bounds.Width / 3);
                    var offset = (bounds.Width + width) * animationFrame / 100 - width;
                    eventArguments.Graphics.FillRectangle(
                        Brushes.SteelBlue,
                        bounds.X + offset,
                        bounds.Y,
                        width,
                        bounds.Height
                    );
                }
                else if (
                    state == "Salvo"
                    || state == "Publicado"
                    || state == "Sucesso"
                    || state == "Verificado no pacote"
                )
                {
                    eventArguments.Graphics.FillRectangle(Brushes.MediumSeaGreen, bounds);
                }
                else if (state == "Falha")
                {
                    eventArguments.Graphics.FillRectangle(Brushes.LightSalmon, bounds);
                }
                else if (state == "Resultado incerto" || OperationResult.IsWarningState(state))
                {
                    eventArguments.Graphics.FillRectangle(Brushes.Khaki, bounds);
                }
                eventArguments.Graphics.Restore(graphicsState);
            }
            eventArguments.Paint(eventArguments.ClipBounds, DataGridViewPaintParts.Border);
            eventArguments.Handled = true;
        }

        private void ReviewFormClosing(object sender, FormClosingEventArgs eventArguments)
        {
            eventArguments.Cancel = running;
            if (running)
            {
                executionStatus.Text = "Aguarde a conclusão das operações em andamento.";
            }
        }
    }
}
