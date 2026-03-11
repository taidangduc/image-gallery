import { Box, Button, Drawer, FileUpload, Icon, Portal, CloseButton } from "@chakra-ui/react";
import type { UseDrawerReturn } from "@chakra-ui/react"
import { VscCloudUpload } from "react-icons/vsc";

type Props = {
    drawer: UseDrawerReturn
}

export function FileUploadDrawer({ drawer }: Props) {
    return (
        <Drawer.Root closeOnInteractOutside={false} modal={false} size="md" {...drawer}>
            <Portal>
                <Drawer.Positioner >
                    <Drawer.Content>
                        <Drawer.Header>
                            <Drawer.Title>Upload File</Drawer.Title>
                        </Drawer.Header>
                        <Drawer.Body>
                            <FileUpload.Root maxW="xl" alignItems="stretch" maxFiles={5} maxFileSize={10 * 1024 * 1024}>
                                <FileUpload.HiddenInput />
                                <FileUpload.Dropzone>
                                    <Icon size="2xl" color="fg.muted">
                                        <VscCloudUpload />
                                    </Icon>
                                    <FileUpload.DropzoneContent>
                                        <Box>Drag and drop files here</Box>
                                        <Box color="fg.muted">.png, .jpg up to 10MB</Box>
                                    </FileUpload.DropzoneContent>
                                </FileUpload.Dropzone>
                                <FileUpload.List showSize clearable />
                            </FileUpload.Root>
                        </Drawer.Body>
                        <Drawer.Footer justifyContent="flex-start" >
                            <Button>Upload</Button>
                            <Drawer.ActionTrigger asChild >
                                <Button variant="outline" onClick={() => drawer.setOpen(false)}>Cancel</Button>
                            </Drawer.ActionTrigger>
                        </Drawer.Footer>
                        <Drawer.CloseTrigger asChild>
                            <CloseButton size="sm" onClick={() => drawer.setOpen(false)} />
                        </Drawer.CloseTrigger>
                    </Drawer.Content>
                </Drawer.Positioner>
            </Portal>
        </Drawer.Root>
    )
}