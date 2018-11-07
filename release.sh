#!/bin/bash

set -e

# https://stackoverflow.com/questions/1885525/how-do-i-prompt-a-user-for-confirmation-in-bash-script
read -p "Are you sure - is it tagged yet? [y|n]? " -n 1 -r
echo    # (optional) move to a new line
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    [[ "$0" = "$BASH_SOURCE" ]] && exit 1 || return 1 # handle exits from shell or function but don't exit interactive shell
fi

echo "build and create a nuget file..."
dotnet pack

echo
echo "Pushing package..."
for f in bin/Debug/*.nupkg
    do
        echo " - Pushing $f ..."
        dotnet nuget push $f
done

echo
echo "Done!"
