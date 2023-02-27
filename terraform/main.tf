# VPC Subnet module: Creates a VPC for our resources to live inside

module "vpc_subnet" {
  source = "git::https://github.com/answerdigital/terraform-modules//Terraform_modules/vpc_subnets?ref=v1.0.0"

  project_name        = var.project_name
  owner               = var.owner
  vpc_cidr            = var.vpc_cidr
  num_public_subnets  = var.num_public_subnets
  num_private_subnets = var.num_private_subnets
}

# Security Group: Defines network traffic rules for our ECS

resource "aws_security_group" "ecs_sg" {
  #checkov:skip=CKV_AWS_260:Allowing ingress from 0.0.0.0 for public HTTP(S) access
  name        = "${var.project_name}-ecs-sg"
  description = "Security group for ECS service"
  vpc_id       = module.vpc_subnet.vpc_id

  ingress {
    from_port       = 0
    to_port         = 0
    protocol        = "-1"
    security_groups = [aws_security_group.alb_sg.id]
    description     = "Application Load Balancer"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
    description = "All traffic"
  }

  tags = {
    Name  = "${var.project_name}-ecs-sg"
    Owner = var.owner
  }
}

# ECS: Elastic Container Service

resource "aws_ecs_cluster" "ecs_cluster" {
  name = "${var.project_name}-ecs-cluster"

  setting {
    name  = "containerInsights"
    value = "enabled"
  }

  tags = {
    Name = "${var.project_name}-ecs-cluster"
    Owner = var.owner
  }
}

resource "aws_ecs_task_definition" "aws_ecs_task" {
  family = "${var.project_name}-task"

  container_definitions = <<DEFINITION
  [
    {
      "name": "${var.project_name}-container",
      "image": "${var.image_url}",
      "entryPoint": [],

      "essential": true,
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "${aws_cloudwatch_log_group.log_group.id}",
          "awslogs-region": "${var.aws_region}",
          "awslogs-stream-prefix": "${var.project_name}"
        }
      },
      "portMappings": [
        {
          "containerPort": 80,
          "hostPort": 80
        },
        {
          "containerPort": 443,
          "hostPort": 443
        }
      ],
      "mountPoints": [
        {
          "sourceVolume": "${var.project_name}-ecs-efs-volume",
          "containerPath": "/app/out/db"
        }
      ],
      "cpu": 256,
      "memory": 512,
      "networkMode": "awsvpc"
    }
  ]
  DEFINITION

  requires_compatibilities = ["FARGATE", "EC2"]
  network_mode             = "awsvpc"
  memory                   = "512"
  cpu                      = "256"

  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn

  volume {
    name = "${var.project_name}-ecs-efs-volume"

    efs_volume_configuration {
      file_system_id          = aws_efs_file_system.persistent.id
      root_directory          = "/app/out/db"
      transit_encryption      = "ENABLED"
      transit_encryption_port = var.efs_port
      authorization_config {
        access_point_id = aws_efs_access_point.access.id
        iam             = "ENABLED"
      }
    }
  }

  tags = {
    Name = "${var.project_name}-ecs-task"
    Owner = var.owner
  }
}

data "aws_ecs_task_definition" "main" {
  task_definition = aws_ecs_task_definition.aws_ecs_task.family
}

resource "aws_ecs_service" "ecs_service" {
  name                 = "${var.project_name}-ecs-service"
  cluster              = aws_ecs_cluster.ecs_cluster.id
  task_definition      = "${aws_ecs_task_definition.aws_ecs_task.family}:${max(aws_ecs_task_definition.aws_ecs_task.revision, data.aws_ecs_task_definition.main.revision)}"
  launch_type          = "FARGATE"
  scheduling_strategy  = "REPLICA"
  platform_version     = "LATEST"
  desired_count        = 1
  force_new_deployment = true

  network_configuration {
    subnets          = module.vpc_subnet.public_subnet_ids
    assign_public_ip = true
    security_groups = [
      aws_security_group.ecs_sg.id,
      aws_security_group.alb_sg.id
    ]
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.alb_target_group.arn
    container_name   = "${var.project_name}-container"
    container_port   =  80
  }
}
