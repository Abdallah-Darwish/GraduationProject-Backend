#!/bin/bash
pg_ctlcluster 13 main start
dotnet /app/GradProjectServer.dll