import * as vscode from 'vscode';
import * as cp from 'child_process';
import * as util from 'util';

const execAsync = util.promisify(cp.exec);

function getPacxPath(): string {
    return vscode.workspace.getConfiguration('pacx').get<string>('path', 'pacx');
}

function getTenantId(): string | undefined {
    const tid = vscode.workspace.getConfiguration('pacx').get<string>('tenantId', '');
    return tid || undefined;
}

async function runPacx(args: string[]): Promise<string> {
    const pacxPath = getPacxPath();
    const cmd = `"${pacxPath}" ${args.map(a => `"${a}"`).join(' ')}`;
    try {
        const { stdout } = await execAsync(cmd, { env: process.env as Record<string, string>, timeout: 60000 });
        return stdout.trim();
    } catch (err: any) {
        throw new Error(`PACX command failed: ${err.stderr || err.message}`);
    }
}

function showOutput(output: string, title: string) {
    const channel = vscode.window.createOutputChannel('PACX');
    channel.clear();
    channel.appendLine(`[${title}]`);
    channel.appendLine('');
    channel.appendLine(output);
    channel.show();
}

async function listForms() {
    const tenantId = getTenantId();
    if (!tenantId) {
        const input = await vscode.window.showInputBox({
            prompt: 'Tenant ID or domain (e.g., contoso.onmicrosoft.com)',
            placeHolder: 'contoso.onmicrosoft.com'
        });
        if (!input) { return; }
        try {
            const output = await runPacx(['forms', 'list', '--tenant', input, '--format', 'table']);
            showOutput(output, `Forms in ${input}`);
        } catch (err: any) {
            vscode.window.showErrorMessage(err.message);
        }
    } else {
        try {
            const output = await runPacx(['forms', 'list', '--tenant', tenantId, '--format', 'table']);
            showOutput(output, `Forms in ${tenantId}`);
        } catch (err: any) {
            vscode.window.showErrorMessage(err.message);
        }
    }
}

async function listSolutions() {
    try {
        const output = await runPacx(['solution', 'list', '--format', 'table']);
        showOutput(output, 'Dataverse Solutions');
    } catch (err: any) {
        vscode.window.showErrorMessage(err.message);
    }
}

async function listEnvironments() {
    try {
        const output = await runPacx(['env', 'capacity', 'report', '--format', 'table']);
        showOutput(output, 'Power Platform Environments');
    } catch (err: any) {
        vscode.window.showErrorMessage(err.message);
    }
}

async function runCommand() {
    const input = await vscode.window.showInputBox({
        prompt: 'PACX command arguments (e.g., forms list --tenant contoso.onmicrosoft.com)',
        placeHolder: 'forms list --tenant contoso.onmicrosoft.com'
    });
    if (!input) { return; }

    const args = input.split(/\s+/).filter(a => a.length > 0);
    try {
        const output = await runPacx(args);
        showOutput(output, `pacx ${input}`);
    } catch (err: any) {
        vscode.window.showErrorMessage(err.message);
    }
}

export function activate(context: vscode.ExtensionContext) {
    context.subscriptions.push(
        vscode.commands.registerCommand('pacx.listForms', listForms),
        vscode.commands.registerCommand('pacx.listSolutions', listSolutions),
        vscode.commands.registerCommand('pacx.listEnvironments', listEnvironments),
        vscode.commands.registerCommand('pacx.runCommand', runCommand)
    );
}

export function deactivate() {}
