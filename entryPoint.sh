#!/bin/sh
echo "Starting GraphQL engine with redirected output"
ls -la ./
graphql-engine serve | ./StdInAppInsightsDispatcher
echo "Done"
