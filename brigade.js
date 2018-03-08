const { events, Job, Group } = require('brigadier')

events.on("push", (brigadeEvent, project) => {
    
    // setup variables
    var gitPayload = JSON.parse(brigadeEvent.payload)
    var brigConfig = new Map()
    brigConfig.set("acrServer", project.secrets.acrServer)
    brigConfig.set("acrUsername", project.secrets.acrUsername)
    brigConfig.set("acrPassword", project.secrets.acrPassword)
    brigConfig.set("webstoreImage", "webstore")
    brigConfig.set("gitSHA", brigadeEvent.commit.substr(0,7))
    brigConfig.set("eventType", brigadeEvent.type)
    brigConfig.set("branch", getBranch(gitPayload))
    brigConfig.set("imageTag", `${brigConfig.get("branch")}-${brigConfig.get("gitSHA")}`)
    brigConfig.set("webstoreACRImage", `${brigConfig.get("acrServer")}/${brigConfig.get("webstoreImage")}`)
    
    console.log(`PUSH ==> gitHub webook (${brigConfig.get("branch")}) with commit ID ${brigConfig.get("gitSHA")}`)
    
    // setup brigade jobs
    var docker = new Job("job-runner-docker")
    var helm = new Job("job-runner-helm")
    dockerJobRunner(brigConfig, docker)
    helmJobRunner(brigConfig, helm)
    // start pipeline
    console.log(`==> starting pipeline for docker image: ${brigConfig.get("webstoreACRImage")}:${brigConfig.get("imageTag")}`)
    var pipeline = new Group()
    pipeline.add(docker)
    pipeline.add(helm)
    if (brigConfig.get("branch") == "master") {
        pipeline.runEach()
    } else {
        console.log(`==> no jobs to run when not master`)
    }  
})

events.on("after", (event, proj) => {
    console.log("brigade pipeline finished successfully")
})

function dockerJobRunner(config, d) {
    d.storage.enabled = false
    d.image = "chzbrgr71/dnd:v5"
    d.privileged = true
    d.tasks = [
        "dockerd-entrypoint.sh &",
        "echo waiting && sleep 20",
        "cd /src/Webstore",
        `docker login ${config.get("acrServer")} -u ${config.get("acrUsername")} -p ${config.get("acrPassword")}`,
        `docker build --build-arg BUILD_DATE='1/1/2017 5:00' --build-arg IMAGE_TAG_REF=${config.get("imageTag")} --build-arg VCS_REF=${config.get("gitSHA")} -t ${config.get("webstoreImage")} .`,
        `docker tag ${config.get("webstoreImage")} ${config.get("webstoreACRImage")}:${config.get("imageTag")}`,
        `docker push ${config.get("webstoreACRImage")}:${config.get("imageTag")}`,
        "killall dockerd"
    ]
}

function helmJobRunner (config, h) {
    h.storage.enabled = false
    h.image = "lachlanevenson/k8s-helm:v2.8.1"
    h.tasks = [
        "cd /src/",
        `helm upgrade --install stickerstore-webstore ./Deployment/Helm/Stickerstore --set image.repository=${config.get("apiACRImage")} --set image.tag=${config.get("imageTag")}`,
    ]
}

function getBranch (p) {
    if (p.ref) {
        return p.ref.substring(11)
    } else {
        return "PR"
    }
}