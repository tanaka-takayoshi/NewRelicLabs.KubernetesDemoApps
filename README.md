# NewRelicLabs.KubernetesDemoApps

Kubernetes環境向けに、負荷をかけたり、外部呼び出しをしたりするデモアプリ

メモリーやCPUの負荷処理は https://github.com/dotnet/samples/tree/master/core/diagnostics/DiagnosticScenarios を参考にしています。

## エンドポイント

- /liveness Liveness Probe
- /readiness Readiness Probe
- /probe?type={readiness|liveness}&disables=true ProbeのON/OFF

- /diag/deadlock
- /diag/highcpu/{milliseconds}
- /diag/memleak/{kb}
- /diag/memspike/{seconds}
- /diag/crash クラッシュ

- /invoke リモート呼び出しのチェーン。環境変数 `REMOVE_URL` で指定されたURLにGETリクエストを投げます。
 
