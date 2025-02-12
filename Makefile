all:

clean:
	find . -type d \( -name "bin" -o -name "obj" -o -name ".vs" -o -name ".vscode" \) -exec rm -rf {} +

deep-clean:
	git rm -r --cached **/bin || echo 'bins not found'
	git rm -r --cached **/obj || echo "objs not found"
	git rm -r --cached **/.vs || echo ".vs not found"
	git rm -r --cached **/.vscode || echo ".vscode not found"

