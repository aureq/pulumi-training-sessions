# Pulumi training using TypeScript (4th session)

Exercises to learn how to use Pulumi (4th session)

## Introduction

This exercise starts where we finished the previous session (a Virtual Network and a VM) but dive deeper on Pulumi reusability patterns at scale. You'll learn how to create your own Component Resource and use stack references along the way.

For a better learning experience, you way want to start with your own code from the previous training session.

## Content

* Create a new stack and install the node modules (`npm install`)
* Create 2 folders named `network` and `app`

### The `network` project

1. Switch to the [`network/`](./exercise/network/) folder, create a new stack and install the node modules (`npm install`)
2. Deploy a fully functional virtual network as a component resource
   * Ensure the component resource is easily portable
3. Determine the necessary stack outputs for the `webserver` project to use

### The `webserver` project

4. Switch to the [`app`](./exercise/webserver/) folder, create a new stack and install the node modules (`npm install`)
5. Use stack references to retrieve the necessary dependencies
5. Use the `get*()` functions to retrieve existing resources
6. Deploy a VM you can SSH into in the previously created subnet
   * Use stack references
   * Use Ubuntu 20.04 LTS (latest version)
   * ensure you can SSH into each VM
7. Create stack outputs for:
   * the VMs hostname
   * the username
   * the password

### Bonus/Challenges

8. Use Pulumi runtime functions to determine the current stack name
9. Your component resource doesn't rely on pulumi.Config()
10. Your component resource args use strong typing

### Resources

* Pulumi [examples](https://github.com/pulumi/examples)

### Answers

You will find all the answers on the azure-ts/session-4-solution branch.
